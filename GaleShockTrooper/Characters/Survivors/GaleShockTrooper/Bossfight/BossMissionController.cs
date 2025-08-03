using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Bossfight
{
    public class BossMissionController : MonoBehaviour
    {
        public static GameObject prefab;
        public static BossMissionController instance;

        public static ItemDef bossStatItem;

        public static CharacterSpawnCard spawnCard;
        public static Vector3 spawnpoint = new Vector3(307f, -140f, 395f);
        public static float spawnDistance = 160f;
        private static float spawnDistanceSqr = spawnDistance * spawnDistance;
        public static GameObject spawnEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/PodGroundImpact.prefab").WaitForCompletion();

        private static GameObject combatGroupPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShadowClone/ShadowCloneEncounter.prefab").WaitForCompletion();
        private GameObject combatGroupInstance;
        private float stopwatch;

        private void Awake()
        {
            stopwatch = 0f;
            transform.position = spawnpoint;
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active || combatGroupInstance) return;

            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= 0.1f)
            {
                stopwatch -= 0.1f;
                CheckForPlayers();
            }
        }

        private void CheckForPlayers()
        {
            foreach (CharacterMaster cm in CharacterMaster.instancesList)
            {
                if (cm.teamIndex != TeamIndex.Player || cm.IsDeadAndOutOfLivesServer()) continue;
                GameObject bodyObject = cm.GetBodyObject();
                if ((transform.position-bodyObject.transform.position).sqrMagnitude <= spawnDistanceSqr)
                {
                    SpawnBossServer();
                }
            }
        }
        
        public void SpawnBossServer()
        {
            if (!NetworkServer.active || !spawnCard) return;
            EffectManager.SimpleEffect(spawnEffectPrefab, spawnpoint, Quaternion.identity, true);


            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
                minDistance = 3f,
                maxDistance = 20f,
                spawnOnTarget = base.transform
            }, RoR2Application.rng);
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.teamIndexOverride = TeamIndex.Monster;

            directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
            {
                if (!spawnResult.spawnedInstance) return;
                Inventory inventory = spawnResult.spawnedInstance.GetComponent<Inventory>();
                if (inventory)
                {
                    //inventory.GiveItem(RoR2Content.Items.InvadingDoppelganger);
                }
            }));

            combatGroupInstance = UnityEngine.Object.Instantiate(combatGroupPrefab);
            CombatSquad cs = combatGroupInstance.GetComponent<CombatSquad>();
            GameObject spawnedObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
            if (spawnedObject)
            {
                CharacterMaster spawnedMaster = spawnedObject.GetComponent<CharacterMaster>();
                if (spawnedMaster)
                {
                    if (cs)
                    {
                        cs.AddMember(spawnedObject.GetComponent<CharacterMaster>());
                    }
                }
            }

            if (cs.memberCount > 0)
            {
                cs.onDefeatedServer += StopMusicOnMissionComplete;
                NetworkServer.Spawn(combatGroupInstance);

                if (!BossMusicController.instance)
                {
                    GameObject mc = UnityEngine.Object.Instantiate(BossMusicController.prefab);
                    var cont = mc.GetComponent<BossMusicController>();
                    if (cont)
                    {
                        BossMusicController.instance = cont;
                        NetworkServer.Spawn(mc);
                        cont.StartMusicServer();
                    }
                }
                else
                {
                    BossMusicController.instance.StartMusicServer();
                }
            }
            else
            {
                Destroy(combatGroupInstance);
                combatGroupInstance = null;
            }
        }

        private void StopMusicOnMissionComplete()
        {
            if (BossMusicController.instance)
            {
                UnityEngine.Object.Destroy(BossMusicController.instance);
            }
            if (instance)
            {
                Destroy(instance);
            }
        }

        private void OnDestroy()
        {
            if (instance && instance == this)
            {
                instance = null;
            }
        }

        internal static void Stage_onServerStageBegin(Stage obj)
        {
            SceneDef sd = SceneCatalog.GetSceneDefForCurrentScene();
            if (sd && sd.baseSceneName == "moon2")
            {
                UnityEngine.Object.Instantiate(prefab);
            }
        }
    }
}

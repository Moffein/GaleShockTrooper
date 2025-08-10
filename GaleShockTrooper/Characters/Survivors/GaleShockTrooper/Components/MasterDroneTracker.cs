using RoR2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Components
{
    public class MasterDroneTracker : MonoBehaviour
    {
        public static int baseMaxDrones = 1;
        public static int maxExtraDrones = 1;

        private Queue<CharacterMaster> summonedDrones = new Queue<CharacterMaster>();
        public CharacterMaster master;

        public static GameObject droneMasterPrefab;
        public static GameObject droneSpawnEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/OmniImpactVFXLoader.prefab").WaitForCompletion();

        public void Awake()
        {
            if (!master) master = base.GetComponent<CharacterMaster>();
        }

        public void SummonDroneServer(Vector3 pos, Quaternion rot)
        {
            if (!NetworkServer.active) return;

            GameObject bodyObject = master.GetBodyObject();
            if (!bodyObject)
            {
                Debug.LogError("SummonDroneServer failed due to null bodyObject.", this);
                return;
            }

            summonedDrones = new Queue<CharacterMaster>(summonedDrones.Where(mst =>
            {
                return mst != null && !mst.IsDeadAndOutOfLivesServer();
            }));

            int max = GetMaxDrones();
            int currentDrones = summonedDrones.Count;
            if (currentDrones >= max)
            {
                int diff = 1 + (currentDrones - max);

                for (int i = 0; i < diff; i++)
                {
                    if (summonedDrones.TryDequeue(out CharacterMaster oldestDrone))
                    {
                        oldestDrone.TrueKill();
                    }
                }
            }

            MasterSummon summon = new MasterSummon
            {
                masterPrefab = droneMasterPrefab,
                position = pos,
                rotation = rot,
                summonerBodyObject = bodyObject,
                ignoreTeamMemberLimit = true,
                inventoryToCopy = master.inventory
            };

            /*CharacterBody masterBody = master.GetBody();
            if (masterBody)
            {
                summon.loadout = new Loadout();
                summon.loadout.bodyLoadoutManager.SetSkinIndex(BodyCatalog.FindBodyIndex("GaleShockTrooperDroneBody"), masterBody.skinIndex);
            }*/

            CharacterMaster droneMaster = summon.Perform();

            if (droneMaster)
            {
                summonedDrones.Enqueue(droneMaster);
                CharacterBody droneBody = droneMaster.GetBody();
                if (droneBody)
                {
                    EffectManager.SimpleEffect(droneSpawnEffect, droneBody.corePosition, Quaternion.identity, true);

                    droneMaster.loadout.bodyLoadoutManager.SetSkinIndex(BodyCatalog.FindBodyIndex("GaleShockTrooperDroneBody"), master.loadout.bodyLoadoutManager.GetSkinIndex(BodyCatalog.FindBodyIndex("GaleShockTrooperBody")));
                    droneMaster.SetLoadoutServer(droneMaster.loadout);
                }
            }
            else
            {
                Debug.LogError("SummonDroneServer failed due to summon master.", this);
            }
        }

        public int GetMaxDrones()
        {
            if (master)
            {
                CharacterBody cb = master.GetBody();
                if (cb && cb.skillLocator && cb.skillLocator.special)
                {
                    return Mathf.Min(cb.skillLocator.special.maxStock, baseMaxDrones + maxExtraDrones);
                }
            }
            return baseMaxDrones;
        }
    }
}

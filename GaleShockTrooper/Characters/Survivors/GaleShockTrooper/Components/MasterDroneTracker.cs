using RoR2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;
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
                int diff = 1 + (max - currentDrones);

                for (int i = 0; i < diff; i++)
                {
                    if (summonedDrones.TryDequeue(out CharacterMaster oldestDrone))
                    {
                        oldestDrone.TrueKill();
                    }
                }
            }

            CharacterMaster characterMaster = new MasterSummon
            {
                masterPrefab = droneMasterPrefab,
                position = pos,
                rotation = rot,
                summonerBodyObject = bodyObject,
                ignoreTeamMemberLimit = true,
                inventoryToCopy = master.inventory
            }.Perform();

            if (characterMaster)
            {
                summonedDrones.Enqueue(characterMaster);
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

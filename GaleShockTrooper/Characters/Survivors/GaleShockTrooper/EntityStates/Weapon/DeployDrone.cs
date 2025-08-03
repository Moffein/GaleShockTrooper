using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Components;

namespace EntityStates.GaleShockTrooperStates.Weapon
{
    public class DeployDrone : BaseState
    {
        public static float baseCooldown = 30f;
        public static float baseDuration = 1f;
        private float duration;
        private bool attemptedSpawn = false;

        public override void OnEnter()
        {
            base.OnEnter();

            characterBody.SetAimTimer(2f);

            duration = baseDuration / attackSpeedStat;
            Util.PlaySound("Play_drone_repair", gameObject);
            PlayAnimation("Gesture, Override", "DeployDrone", "Shootgun.playbackRate", duration);

            AttemptSpawnDroneSerer();
        }

        private void AttemptSpawnDroneSerer()
        {
            if (!NetworkServer.active || attemptedSpawn) return;

            if (characterBody && characterBody.master)
            {
                attemptedSpawn = true;
                MasterDroneTracker mdt = characterBody.master.GetComponent<MasterDroneTracker>();
                if (!mdt)
                {
                    mdt = characterBody.master.gameObject.AddComponent<MasterDroneTracker>();
                    mdt.master = characterBody.master;
                }

                Ray aimRay = GetAimRay();
                Vector3 dir = aimRay.direction;
                dir.y = 0f;
                dir.Normalize();

                Vector3 spawnPos = characterBody.corePosition + Vector3.up * 2.5f + dir * 2.5f;

                mdt.SummonDroneServer(spawnPos, Util.QuaternionSafeLookRotation(dir));
            }
            else
            {
                Debug.LogError("CharacterBody has no master, failed to deploy drone!", GaleShockTrooper.GaleShockTrooperPlugin.instance);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            if (!outer.destroying) AttemptSpawnDroneSerer();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}

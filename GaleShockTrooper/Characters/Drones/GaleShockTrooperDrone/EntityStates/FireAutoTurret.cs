using RoR2;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;
using UnityEngine.AddressableAssets;
using EntityStates.GaleShockTrooperStates.Weapon;
using RoR2.Orbs;

namespace EntityStates.GaleShockTrooperDroneStates
{
    public class FireAutoTurret : BaseState
    {
        public static float damageCoefficient = 1f;
        public static float lockonAngle = 60f;
        public static float baseShotDuration = 0.15f;
        public static float baseDuration = 0.5f;
        public static int shotsPerBurst = 2;
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
        public static GameObject orbEffectPrefab;
        public static float lockonRange = 90f;

        private float shotDuration, duration, stopwatch;
        private int shotsFired;
        private bool isCrit;

        private bool rightMuzzle;
        private BullseyeSearch search;

        private Transform muzzleL, muzzleR;
        private HurtBox currentTarget;

        public override void OnEnter()
        {
            base.OnEnter();
            characterBody.SetAimTimer(2f);
            rightMuzzle = true;
            isCrit = RollCrit();
            shotsFired = 0;
            shotDuration = baseShotDuration / attackSpeedStat;
            duration = baseDuration / attackSpeedStat;
            stopwatch = 0f;
            search = new BullseyeSearch();
            ChildLocator cl = GetModelChildLocator();
            if (cl)
            {
                muzzleR = cl.FindChild("MuzzleR");
                muzzleL = cl.FindChild("MuzzleL");
            }
            FireShot();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            stopwatch += GetDeltaTime();
            if (stopwatch >= shotDuration && shotsFired < shotsPerBurst)
            {
                stopwatch -= shotDuration;
                FireShot();
            }

            if (isAuthority && fixedAge >= duration && shotsFired >= shotsPerBurst)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            if (shotsFired < shotsPerBurst)
            {
                int toShoot = shotsPerBurst - shotsFired;
                for (int i = 0; i < toShoot; i++)
                {
                    FireShot(false);
                }
            }
            base.OnExit();
        }

        private void FireShot(bool playEffects = true)
        {
            shotsFired++;
            string muzzleString = rightMuzzle ? "MuzzleR" : "MuzzleL";
            Transform muzzleTransform = transform;
            if (rightMuzzle && muzzleR)
            {
                muzzleTransform = muzzleR;
            }
            else if (!rightMuzzle && muzzleL)
            {
                muzzleTransform = muzzleL;
            }
            rightMuzzle = !rightMuzzle;

            if (playEffects)
            {
                EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, muzzleString, false);
                Util.PlaySound("Play_engi_R_turret_shot", gameObject);
            }

            if (NetworkServer.active)
            {
                FireOrb(muzzleTransform);
            }
        }

        private void FireOrb(Transform muzzleTransform)
        {
            Ray aimRay = GetAimRay();
            if (!(currentTarget && currentTarget.healthComponent && currentTarget.healthComponent.alive))
            {
                search.teamMaskFilter = TeamMask.all;
                search.teamMaskFilter.RemoveTeam(GetTeam());
                search.filterByLoS = true;
                search.searchOrigin = aimRay.origin;
                search.searchDirection = aimRay.direction;
                search.sortMode = BullseyeSearch.SortMode.Angle;
                search.maxDistanceFilter = lockonRange;
                search.maxAngleFilter = lockonAngle;
                search.RefreshCandidates();
                search.FilterOutGameObject(base.gameObject);
                currentTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
            }
            if (!currentTarget) return;

            ChainGunOrb chainGunOrb = new ChainGunOrb(FireRicochetSlug.orbEffectPrefab);
            chainGunOrb.damageValue = damageCoefficient * damageStat;
            chainGunOrb.isCrit = isCrit;
            chainGunOrb.teamIndex = GetTeam();
            chainGunOrb.attacker = gameObject;
            chainGunOrb.procCoefficient = 1f;
            chainGunOrb.procChainMask = default;
            chainGunOrb.origin = muzzleTransform.position;
            chainGunOrb.speed = 200f;
            chainGunOrb.bouncesRemaining = 0;
            chainGunOrb.bounceRange = 60f;
            chainGunOrb.damageCoefficientPerBounce = 1f;
            chainGunOrb.targetsToFindPerBounce = 1;
            chainGunOrb.canBounceOnSameTarget = false;
            chainGunOrb.damageColorIndex = DamageColorIndex.Default;
            chainGunOrb.damageType = DamageTypeCombo.GenericSpecial;

            chainGunOrb.target = currentTarget;
            OrbManager.instance.AddOrb(chainGunOrb);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

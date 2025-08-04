using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.GaleShockTrooperStates.Weapon
{
    public class ThrowSticky : BaseState
    {
        public static int baseMaxStocks = 1;
        public static float baseCooldown = 6;

        public static float damageCoefficient = 3f;
        public static float blastRadius = 12f;
        public static float baseDuration = 0.6f;
        public static float detonationDelay = 1.5f;

        public static GameObject projectilePrefab;
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion();

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound("Play_MULT_m1_grenade_launcher_shoot", gameObject);
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, "Muzzle", false);
            PlayAnimation("Gesture, Override", "ShootSticky", "Shootgun.playbackRate", duration);
            if (isAuthority)
            {
                Ray aimRay = GetAimRay();
                FireProjectileInfo fpi = new FireProjectileInfo
                {
                    crit = RollCrit(),
                    damage = damageStat * damageCoefficient,
                    force = 400f,
                    owner = gameObject,
                    position = aimRay.origin,
                    projectilePrefab = projectilePrefab,
                    procChainMask = default,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    damageTypeOverride = (DamageTypeCombo) DamageType.AOE | DamageSource.Secondary,
                    speedOverride = 80f
                };
                ProjectileManager.instance.FireProjectile(fpi);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}

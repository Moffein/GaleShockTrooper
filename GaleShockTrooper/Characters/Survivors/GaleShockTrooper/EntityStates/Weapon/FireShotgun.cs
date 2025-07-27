using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.GaleShockTrooperStates.Weapon
{
    public class FireShotgun : BaseState
    {
        public static float maxRange = 80f;
        public static uint pelletCount = 5;
        public static float damageCoefficient = 0.4f;
        public static float procCoefficient = 0.6f;
        public static float baseDuration = 0.45f;
        public static float force = 100f;
        public static float spread = 3f;
        public static float recoil = 1f;
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommandoShotgun.prefab").WaitForCompletion();
        public static GameObject tracerEffectPrefab;
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;

            Util.PlaySound("Play_captain_m1_shootWide", gameObject);
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, "Muzzle", false);
            PlayAnimation("Gesture, Override", "ShootGun", "Shootgun.playbackRate", duration);

            if (isAuthority)
            {
                Ray aimRay = GetAimRay();
                new BulletAttack
                {
                    damage = damageStat * damageCoefficient,
                    procChainMask = default,
                    procCoefficient = procCoefficient,
                    maxDistance = maxRange,
                    hitEffectPrefab = hitEffectPrefab,
                    tracerEffectPrefab = tracerEffectPrefab,
                    bulletCount = pelletCount,
                    damageType = DamageTypeCombo.GenericPrimary,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    force = force,
                    isCrit = RollCrit(),
                    muzzleName = "Muzzle",
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    minSpread = 0f,
                    maxSpread = spread,
                    owner = gameObject,
                    radius = 0.3f,
                    smartCollision = true
                }.Fire();

            }
            AddRecoil(-recoil, recoil, -0.5f * recoil, 0.5f * recoil);
            characterBody.AddSpreadBloom(0.4f);
            characterBody.SetAimTimer(2f);
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
            return InterruptPriority.Skill;
        }
    }
}

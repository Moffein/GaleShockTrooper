using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.GaleShockTrooperStates.Weapon
{
    public class FireRicochetSlug : BaseState
    {
        public static float ricochetRange = 45f;
        public static float damageCoefficient = 10f;
        public static float baseDuration = 0.6f;
        public static float force = 2000f;
        public static float recoil = 3f;
        public static int ricochetCount = 9;

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/ImpactRailgun.prefab").WaitForCompletion();
        public static GameObject tracerEffectPrefab;
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/MuzzleflashBandit2.prefab").WaitForCompletion();
        public static GameObject orbEffectPrefab;
        public static GameObject ricochetImpactEffect;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;

            Util.PlaySound("Play_bandit_M2_shot", gameObject);
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, "Muzzle", false);
            PlayAnimation("Gesture, Override", "ShootSlug", "Shootgun.playbackRate", duration);

            if (isAuthority)
            {
                Ray aimRay = GetAimRay();
                BulletAttack ba = new BulletAttack
                {
                    damage = damageStat * damageCoefficient,
                    procChainMask = default,
                    procCoefficient = 1f,
                    maxDistance = 2000f,
                    hitEffectPrefab = hitEffectPrefab,
                    tracerEffectPrefab = tracerEffectPrefab,
                    bulletCount = 1,
                    damageType = DamageTypeCombo.GenericSpecial,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    force = force,
                    isCrit = RollCrit(),
                    muzzleName = "Muzzle",
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    minSpread = 0f,
                    maxSpread = 0f,
                    owner = gameObject,
                    radius = 2f,
                    smartCollision = true
                };
                ba.damageType.AddModdedDamageType(GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content.CharacterDamageTypes.SpecialSlugProc);
                ba.damageType.AddModdedDamageType(GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content.CharacterDamageTypes.SpecialSlugVisual);
                ba.Fire();

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
            return InterruptPriority.PrioritySkill;
        }
    }
}

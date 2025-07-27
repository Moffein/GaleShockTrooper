using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.GaleShockTrooperStates.Weapon.MissilePainter
{
    public class FireMissiles : BaseState
    {
        public static float baseDuration = 0.12f;
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 4f;
        public static float force = 360f;
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();

        private float duration;
        private bool clearTargetList = true;
        public bool isCrit = false;

        public int maxAttacks;  //This determines how many missiles to fire, in case of a desync between TargetList and the actual attacks.
        public int attacksFired;
        public List<PaintMissiles.TargetInfo> targetList;


        public override void OnEnter()
        {
            base.OnEnter();
            if (targetList == null) targetList = new List<PaintMissiles.TargetInfo>();
            if (characterBody) characterBody.SetAimTimer(2f);
            duration = baseDuration / attackSpeedStat;

            if (skillLocator && skillLocator.secondary)
            {
                if (characterBody)
                {
                    characterBody.OnSkillActivated(skillLocator.secondary);
                    if (NetworkServer.active)
                    {
                        HandleLuminousShotServer(characterBody);
                    }
                }
                if (isAuthority) skillLocator.secondary.DeductStock(1);
            }

            FireMissile();
        }

        internal static void HandleLuminousShotServer(CharacterBody body)
        {
            if (!NetworkServer.active || !body || !body.inventory) return;
            if (body.inventory.GetItemCount(DLC2Content.Items.IncreasePrimaryDamage) <= 0) return;

            body.AddIncreasePrimaryDamageStack();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority && fixedAge >= duration)
            {
                if (attacksFired >= maxAttacks)
                {
                    outer.SetNextStateToMain();
                }
                else
                {
                    clearTargetList = false;
                    outer.SetNextState(new FireMissiles
                    {
                        targetList = this.targetList,
                        attacksFired = this.attacksFired,
                        maxAttacks = this.maxAttacks,
                        isCrit = this.isCrit
                    });
                }
            }
        }

        public override void OnExit()
        {
            if (clearTargetList)
            {
                foreach (PaintMissiles.TargetInfo tInfo in targetList)
                {
                    tInfo.indicator.active = false;
                    tInfo.indicator.DestroyVisualizer();
                }
            }
            base.OnExit();
        }

        private void FireMissile()
        {
            attacksFired++;
            Util.PlaySound("Play_GaleShockTrooper_MicroMissile", gameObject);
            PlayAnimation("Gesture, Override", "Missile_Shoot", "Shootgun.playbackRate", duration);
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, "Muzzle", false);
            EffectManager.SimpleMuzzleFlash(PaintMissiles.smokeEffectPrefab, gameObject, "VentL", false);
            EffectManager.SimpleMuzzleFlash(PaintMissiles.smokeEffectPrefab, gameObject, "VentR", false);
            if (isAuthority)
            {
                GameObject target = null;
                PaintMissiles.TargetInfo info = targetList.FirstOrDefault();
                if (info != null)
                {
                    info.SetTargetCount(info.GetTargetCount() - 1);
                    target = info.hurtBox.gameObject;
                }


                Ray aimRay = GetAimRay();

                //ICBM
                int icbmCount = 0;
                if (characterBody && characterBody.inventory)
                {
                    icbmCount = characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                }

                if (icbmCount > 0)
                {
                    Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                    Vector3 axis = Vector3.Cross(aimRay.direction, rhs);
                    float currentSpread = 0f;
                    float angle = 0f;
                    float num2 = 0f;
                    num2 = UnityEngine.Random.Range(1f + currentSpread, 1f + currentSpread) * 3f;   //Bandit is x2
                    angle = num2 / 2f;  //3 - 1 rockets

                    Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
                    Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                    Ray aimRay2 = new Ray(aimRay.origin, direction);
                    for (int i = 0; i < 3; i++)
                    {
                        FireProjectileInfo fpi = new FireProjectileInfo
                        {
                            damage = damageCoefficient * damageStat,
                            damageTypeOverride = DamageTypeCombo.GenericSecondary,
                            crit = isCrit,
                            force = force,
                            owner = gameObject,
                            position = aimRay.origin,
                            procChainMask = default,
                            projectilePrefab = projectilePrefab,
                            rotation = Util.QuaternionSafeLookRotation(aimRay2.direction),
                            target = target
                        };
                        fpi.damage *= 1f + 0.5f * (icbmCount - 1);

                        ProjectileManager.instance.FireProjectile(fpi);
                        aimRay2.direction = rotation * aimRay2.direction;
                    }
                }
                else
                {
                    FireProjectileInfo fpi = new FireProjectileInfo
                    {
                        damage = damageCoefficient * damageStat,
                        damageTypeOverride = DamageTypeCombo.GenericSecondary,
                        crit = isCrit,
                        force = force,
                        owner = gameObject,
                        position = aimRay.origin,
                        procChainMask = default,
                        projectilePrefab = projectilePrefab,
                        rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                        target = target
                    };
                    ProjectileManager.instance.FireProjectile(fpi);
                }

                targetList = targetList.Where(tInfo => tInfo.GetTargetCount() > 0).ToList();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}

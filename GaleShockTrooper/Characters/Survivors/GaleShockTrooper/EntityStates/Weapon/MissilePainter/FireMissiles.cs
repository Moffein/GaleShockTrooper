using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        public bool isCrit;

        public int maxAttacks;  //This determines how many missiles to fire, in case of a desync between TargetList and the actual attacks.
        public int attacksFired;
        public List<PaintMissiles.TargetInfo> targetList;


        public override void OnEnter()
        {
            base.OnEnter();
            if (characterBody) characterBody.SetAimTimer(2f);
            duration = baseDuration / attackSpeedStat;
            isCrit = RollCrit();
            FireMissile();
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
            if (isAuthority)
            {
                if (skillLocator && skillLocator.secondary)
                {
                    skillLocator.secondary.DeductStock(1);

                    if (characterBody)
                    {
                        characterBody.OnSkillActivated(skillLocator.secondary);
                    }
                }

                GameObject target = null;
                PaintMissiles.TargetInfo info = targetList.FirstOrDefault();
                if (info != null)
                {
                    info.SetTargetCount(info.GetTargetCount() - 1);
                    target = info.hurtBox.gameObject;
                }
                //Fire Missile while reading from info if it is notnull

                Ray aimRay = GetAimRay();
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
                targetList = targetList.Where(tInfo => tInfo.GetTargetCount() > 0).ToList();
            }
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, "Muzzle", false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

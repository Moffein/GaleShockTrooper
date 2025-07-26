using RoR2;
using RoR2.Skills;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.GaleShockTrooperStates.Weapon.MissilePainter
{
    public class PaintMissiles : BaseState
    {
        public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerCrosshair.prefab").WaitForCompletion();
        public static SkillDef primaryOverride;
        public static string entrySoundString = "Play_railgunner_m2_scope_in";
        public static string exitSoundString = "Play_railgunner_m2_scope_out";
        public static float baseEntryduration = 0.25f;

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private GenericSkill overriddenSkill;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(entrySoundString, gameObject);
            Util.PlaySound("Play_railgunner_m2_scope_loop", gameObject);
            PlayAnimation("Gesture, Override", "Missile_Start", "Shootgun.playbackRate", baseEntryduration/attackSpeedStat);
            this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(characterBody, crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);



            /*GenericSkill genericSkill = (skillLocator != null) ? skillLocator.primary : null;
            if (genericSkill)
            {
                this.TryOverrideSkill(genericSkill);
                genericSkill.onSkillChanged += this.TryOverrideSkill;
            }*/
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                bool shouldExit = false;
                if (inputBank && !base.inputBank.skill2.down)
                {
                    shouldExit = true;
                }

                if (shouldExit)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            /*
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.primary : null;
            if (genericSkill)
            {
                genericSkill.onSkillChanged -= this.TryOverrideSkill;
            }
            if (this.overriddenSkill)
            {
                this.overriddenSkill.UnsetSkillOverride(this, primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
            }*/

            if (this.crosshairOverrideRequest != null)
            {
                this.crosshairOverrideRequest.Dispose();
            }

            Util.PlaySound("Stop_railgunner_m2_scope_loop", base.gameObject);
            Util.PlaySound(exitSoundString, base.gameObject);

            if (!outer.destroying)
            {
                PlayAnimation("Gesture, Override", "BufferEmpty", "Shootgun.playbackRate", baseEntryduration / attackSpeedStat);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        //Copied from Railgunner
        private void TryOverrideSkill(GenericSkill skill)
        {
            if (skill && !this.overriddenSkill && !skill.HasSkillOverrideOfPriority(GenericSkill.SkillOverridePriority.Contextual))
            {
                this.overriddenSkill = skill;
                this.overriddenSkill.SetSkillOverride(this, primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
                this.overriddenSkill.stock = base.skillLocator.secondary.stock;
            }
        }
    }
}

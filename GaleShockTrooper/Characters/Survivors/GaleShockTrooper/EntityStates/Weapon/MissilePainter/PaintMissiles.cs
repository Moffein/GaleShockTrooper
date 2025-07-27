using RoR2;
using RoR2.Skills;
using RoR2.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.GaleShockTrooperStates.Weapon.MissilePainter
{
    public class PaintMissiles : BaseState
    {
        public static GameObject missileTrackingIndicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMissileTrackingIndicator.prefab").WaitForCompletion();
        public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerCrosshair.prefab").WaitForCompletion();
        public static SkillDef primaryOverride = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiConfirmTargetDummy.asset").WaitForCompletion();
        public static string entrySoundString = "Play_railgunner_m2_scope_in";
        public static string exitSoundString = "Play_railgunner_m2_scope_out";
        public static float baseEntryduration = 0.3f;
        public static float baseLockonDuration = 0.4f;
        public static float baseLockonAngle = 60f;
        public static float baseLockonRange = 200f;

        private Indicator generalIndicator;
        private float lockonDuration;
        private float lockonStopwatch;

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private GenericSkill overriddenSkill;
        public BullseyeSearch search;

        private HurtBox lockonTarget;
        public List<TargetInfo> targetList;

        private bool startedPainting = false;   //Jank for keeping track of whether to run missile painting code
        private bool clearTargetList = true;    // Clear targeting indicators if this is true. Set to false when going to FireMissiles state.

        private bool buttonReleased = false;
        private bool buttonRepressed = false;

        public class TargetInfo
        {
            public HurtBox hurtBox;
            public STMissileIndicator indicator;
            private int targetCount;

            public TargetInfo(GameObject owner, HurtBox hurtBox)
            {
                this.hurtBox = hurtBox;
                indicator = new STMissileIndicator(owner, missileTrackingIndicator);
                indicator.targetTransform = hurtBox.transform;
                indicator.active = true;
                targetCount = 1;
            }

            public int GetTargetCount()
            {
                return targetCount;
            }

            public void SetTargetCount(int i)
            {
                targetCount = i;
                indicator.missileCount = targetCount;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            search = new BullseyeSearch();
            generalIndicator = new Indicator(gameObject, missileTrackingIndicator);
            targetList = new List<TargetInfo>();
            lockonStopwatch = 0f;
            lockonDuration = baseLockonDuration / attackSpeedStat;
            Util.PlaySound(entrySoundString, gameObject);
            Util.PlaySound("Play_railgunner_m2_scope_loop", gameObject);
            PlayAnimation("Gesture, Override", "Missile_Start", "Shootgun.playbackRate", baseEntryduration/attackSpeedStat);
            this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(characterBody, crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);

            if (characterBody) characterBody.SetAimTimer(2f);

            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.primary : null;
            if (genericSkill)
            {
                this.TryOverrideSkill(genericSkill);
                genericSkill.onSkillChanged += this.TryOverrideSkill;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterBody) characterBody.SetAimTimer(2f);
            if (base.isAuthority)
            {
                if (GetCurrentTargets() < GetMaxTargets())
                {
                    UpdateTrackerAuthority();
                    if (startedPainting)
                    {
                        UpdatePainterAuthority();
                    }
                }
                else
                {
                    generalIndicator.targetTransform = null;
                    generalIndicator.active = false;
                }

                bool shouldExit = false;
                if (inputBank)
                {
                    if (!startedPainting && base.inputBank.skill1.down)
                    {
                        startedPainting = true;
                    }
                    else if (startedPainting && !base.inputBank.skill1.down)
                    {
                        if (targetList.Count > 0)
                        {
                            outer.SetNextState(new FireMissiles
                            {
                                attacksFired = 0,
                                targetList = this.targetList,
                                maxAttacks = GetCurrentTargets()
                            });
                            return;
                        }
                        else
                        {
                            lockonStopwatch = 0f;
                            startedPainting = false;
                        }
                    }
                    
                    //Jank for making this skill a toggle skill
                    if (!buttonReleased && !base.inputBank.skill2.down)
                    {
                        buttonReleased = true;
                    }
                    else if (buttonReleased && base.inputBank.skill2.down)
                    {
                        buttonRepressed = true;
                    }
                    else if (buttonRepressed && buttonRepressed && !base.inputBank.skill2.down)
                    {
                        shouldExit = true;
                    }

                    if (characterBody && characterBody.isSprinting) shouldExit = true;
                }

                if (shouldExit)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        private int GetMaxTargets()
        {
            if (skillLocator && skillLocator.secondary) return skillLocator.secondary.stock;
            return 3;
        }

        private int GetCurrentTargets()
        {
            int count = 0;
            foreach (TargetInfo tInfo in targetList)
            {
                count += tInfo.GetTargetCount();
            }
            return count;
        }

        private void UpdateTrackerAuthority()
        {
            HurtBox oldTarget = lockonTarget;
            SearchForTarget(GetAimRay());

            if (lockonTarget)
            {
                generalIndicator.targetTransform = lockonTarget.transform;
                generalIndicator.active = true;

                foreach (TargetInfo tInfo in targetList)
                {
                    if (tInfo.hurtBox == lockonTarget)
                    {
                        generalIndicator.active = false;
                        break;
                    }
                }
            }
            else
            {
                generalIndicator.targetTransform = null;
                generalIndicator.active = false;
            }

            if (lockonTarget != oldTarget)
            {
                lockonStopwatch = lockonTarget ? lockonDuration * 0.75f : 0f;
                return;
            }
        }

        private void UpdatePainterAuthority()
        {
            if (!lockonTarget) return;
            lockonStopwatch += GetDeltaTime();
            if (lockonStopwatch >= lockonDuration)
            {
                lockonStopwatch -= lockonDuration;
                AddLockonTarget();
            }
        }

        private void AddLockonTarget()
        {
            if (!lockonTarget) return;

            Util.PlaySound("Play_engi_seekerMissile_lockOn", gameObject);
            bool setLock = false;
            //Check if target is in list already
            foreach (TargetInfo tInfo in targetList)
            {
                if (tInfo.hurtBox == lockonTarget)
                {
                    setLock = true;
                    tInfo.SetTargetCount(tInfo.GetTargetCount() + 1);
                }
            }

            //If not, add to list
            if (!setLock)
            {
                TargetInfo info = new TargetInfo(gameObject, lockonTarget);
                targetList.Add(info);
            }
        }

        public override void OnExit()
        {
            generalIndicator.active = false;
            generalIndicator.DestroyVisualizer();

            if (clearTargetList)
            {
                foreach (TargetInfo tInfo in targetList)
                {
                    tInfo.indicator.active = false;
                    tInfo.indicator.DestroyVisualizer();
                }
            }
            
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.primary : null;
            if (genericSkill)
            {
                genericSkill.onSkillChanged -= this.TryOverrideSkill;
            }
            if (this.overriddenSkill)
            {
                this.overriddenSkill.UnsetSkillOverride(this, primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
            }

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

        private void SearchForTarget(Ray aimRay)
        {
            search.teamMaskFilter = TeamMask.all;
            search.teamMaskFilter.RemoveTeam(GetTeam());
            search.filterByLoS = true;
            search.searchOrigin = aimRay.origin;
            search.searchDirection = aimRay.direction;
            search.sortMode = BullseyeSearch.SortMode.Angle;
            search.maxDistanceFilter = baseLockonRange;
            search.maxAngleFilter = baseLockonAngle;
            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);
            lockonTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        private void TryOverrideSkill(GenericSkill skill)
        {
            if (skill && !this.overriddenSkill && !skill.HasSkillOverrideOfPriority(GenericSkill.SkillOverridePriority.Contextual))
            {
                this.overriddenSkill = skill;
                this.overriddenSkill.SetSkillOverride(this, primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
                this.overriddenSkill.stock = base.skillLocator.secondary.stock;
            }
        }

        public class STMissileIndicator : Indicator
        {
            // Token: 0x06001732 RID: 5938 RVA: 0x000A5A5C File Offset: 0x000A3C5C
            public override void UpdateVisualizer()
            {
                base.UpdateVisualizer();
                Transform transform = base.visualizerTransform.Find("DotOrigin");
                for (int i = transform.childCount - 1; i >= this.missileCount; i--)
                {
                    EntityState.Destroy(transform.GetChild(i).gameObject);
                }
                for (int j = transform.childCount; j < this.missileCount; j++)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(base.visualizerPrefab.transform.Find("DotOrigin/DotTemplate").gameObject, transform);
                    base.FindRenderers(gameObject.transform);
                }
                if (transform.childCount > 0)
                {
                    float num = 360f / (float)transform.childCount;
                    float num2 = (float)(transform.childCount - 1) * 90f;
                    for (int k = 0; k < transform.childCount; k++)
                    {
                        Transform child = transform.GetChild(k);
                        child.gameObject.SetActive(true);
                        child.localRotation = Quaternion.Euler(0f, 0f, num2 + (float)k * num);
                    }
                }
            }

            // Token: 0x06001733 RID: 5939 RVA: 0x00011F95 File Offset: 0x00010195
            public STMissileIndicator(GameObject owner, GameObject visualizerPrefab) : base(owner, visualizerPrefab)
            {
            }

            // Token: 0x04001E35 RID: 7733
            public int missileCount;
        }
    }
}

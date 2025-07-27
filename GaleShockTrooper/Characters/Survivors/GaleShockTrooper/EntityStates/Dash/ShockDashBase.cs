using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using RoR2.Orbs;

namespace EntityStates.GaleShockTrooperStates.Dash
{
    public class ShockDashBase : BaseState
    {
        public static float shockRange = 12f;
        public static float shockDamageCoefficient = 1f;
        public static int shockTicksPerSecond = 30;
        public static float baseSpeed = 8f;

        public static float baseDuration = 0.3f;
        public static GameObject blinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion();

        public static Material material1 = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashBright.mat").WaitForCompletion();
        public static Material material2 = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashExpanded.mat").WaitForCompletion();

        public Vector3 blinkVector;

        private float shockTickStopwatch;
        private Transform modelTransform;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;
        private List<HealthComponent> victimList;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound("Play_huntress_shift_mini_blink", gameObject);
            PlayDashAnim();
            //StartAimMode(GetAimRay(), baseDuration + 0.3f);
            CreateBlinkEffect(Util.GetCorePosition(gameObject));

            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();

                CharacterModel cm = modelTransform.GetComponent<CharacterModel>();
                if (cm)
                {
                    TemporaryOverlayInstance temporaryOverlay = TemporaryOverlayManager.AddOverlay(cm.gameObject);
                    temporaryOverlay.duration = 0.6f + baseDuration;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = material1;
                    temporaryOverlay.AddToCharacterModel(cm);
                    temporaryOverlay.Start();


                    TemporaryOverlayInstance temporaryOverlay2 = TemporaryOverlayManager.AddOverlay(cm.gameObject);
                    temporaryOverlay2.duration = 0.7f + baseDuration;
                    temporaryOverlay2.animateShaderAlpha = true;
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay2.destroyComponentOnEnd = true;
                    temporaryOverlay2.originalMaterial = material2;
                    temporaryOverlay2.AddToCharacterModel(cm);
                    temporaryOverlay2.Start();
                }
            }

            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }

            if (NetworkServer.active)
            {
                victimList = new List<HealthComponent>();
                ShockEnemiesServer();
                shockTickStopwatch = 0f;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                shockTickStopwatch += Time.fixedDeltaTime;
                if (shockTickStopwatch >= 1f / shockTicksPerSecond)
                {
                    ShockEnemiesServer();
                }
            }
            if (isAuthority)
            {
                DashPhysics();
                if (fixedAge >= baseDuration)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!outer.destroying)
            {
                CreateBlinkEffect(Util.GetCorePosition(gameObject));
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public virtual void ShockEnemiesServer()
        {
            if (!NetworkServer.active) return;
            if (shockTickStopwatch != 0f) shockTickStopwatch -= 1f / shockTicksPerSecond;

            TeamIndex teamIndex = GetTeam();
            Vector3 pos = Util.GetCorePosition(gameObject);

            var detectedEnemies = GaleShockTrooper.Utils.FindEnemiesInSphere(shockRange, pos, teamIndex);
            foreach (HealthComponent hc in detectedEnemies)
            {
                if (victimList.Contains(hc)) continue;
                victimList.Add(hc);

                LightningOrb lightning = new LightningOrb
                {
                    attacker = gameObject,
                    inflictor = gameObject,
                    damageValue = damageStat * shockDamageCoefficient,
                    procCoefficient = 1f,
                    teamIndex = teamIndex,
                    isCrit = RollCrit(),
                    procChainMask = default,
                    lightningType = LightningOrb.LightningType.Ukulele,
                    damageColorIndex = DamageColorIndex.Default,
                    bouncesRemaining = 0,
                    targetsToFindPerBounce = 1,
                    range = shockRange,
                    origin = pos,
                    damageType = new DamageTypeCombo()
                    {
                        damageType = DamageType.Shock5s,
                        damageSource = DamageSource.Utility
                    },
                    speed = 120f,
                    target = hc.body.mainHurtBox
                };
                OrbManager.instance.AddOrb(lightning);
            }
        }

        public virtual void PlayDashAnim()
        {
            PlayAnimation("FullBody, Override", "DashF", "Dash.playbackRate", baseDuration);
        }

        public virtual void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData
            {
                rotation = Util.QuaternionSafeLookRotation(blinkVector),
                origin = origin
            };
            EffectManager.SpawnEffect(blinkPrefab, effectData, false);
        }

        public virtual void DashPhysics()
        {
            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion += blinkVector * (moveSpeedStat * baseSpeed * Time.deltaTime);
            }
        }
    }

    //These are needed since server won't know the player input direction.
    public class ShockDashR : ShockDashBase
    {
        public override void PlayDashAnim()
        {
            PlayAnimation("FullBody, Override", "DashR", "Dash.playbackRate", baseDuration);
        }
    }
    public class ShockDashL : ShockDashBase
    {
        public override void PlayDashAnim()
        {
            PlayAnimation("FullBody, Override", "DashL", "Dash.playbackRate", baseDuration);
        }
    }
    public class ShockDashB : ShockDashBase
    {
        public override void PlayDashAnim()
        {
            PlayAnimation("FullBody, Override", "DashB", "Dash.playbackRate", baseDuration);
        }
    }
}

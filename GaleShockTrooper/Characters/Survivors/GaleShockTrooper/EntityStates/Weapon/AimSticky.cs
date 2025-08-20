using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.GaleShockTrooperStates.Weapon
{
    public class AimSticky : AimThrowableBase
    {
        public static GameObject aimEndpointVisualizerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRainIndicator.prefab").WaitForCompletion();
        public static GameObject aimArcVisualizerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BasicThrowableVisualizer.prefab").WaitForCompletion();

        private float duration;
        public override void OnEnter()
        {
            maxDistance = 60;
            rayRadius = 2f;
            arcVisualizerPrefab = aimArcVisualizerPrefab;
            projectilePrefab = ThrowSticky.projectilePrefab;
            endpointVisualizerPrefab = aimEndpointVisualizerPrefab;
            endpointVisualizerRadiusScale = ThrowSticky.blastRadius;
            setFuse = false;
            baseMinimumDuration = 0f;
            projectileBaseSpeed = ThrowSticky.projectileSpeed;
            damageCoefficient = ThrowSticky.damageCoefficient;

            base.OnEnter();
            duration = ThrowSticky.baseDuration / this.attackSpeedStat;
            PlayAnimation("Gesture, Override", "Missile_Start", "Shootgun.playbackRate", duration * 0.5f);
            StartAimMode();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
        }

        public override void OnExit()
        {
            PlayAnimation("Gesture, Override", "Missile_Shoot", "Shootgun.playbackRate", duration);
            Util.PlaySound("Play_MULT_m1_grenade_launcher_shoot", gameObject);
            StartAimMode();
            base.OnExit();
        }
    }
}

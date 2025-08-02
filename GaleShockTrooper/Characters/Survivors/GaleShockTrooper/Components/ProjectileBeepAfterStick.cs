using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Components
{
    public class ProjectileBeepAfterStick : MonoBehaviour
    {
        public int timesToBeep;
        private int timesBeeped;

        private float beepDuration;
        private float beepStopwatch;
        private ProjectileImpactExplosion pie;
        public GameObject beepEffectPrefab;

        private void Start()
        {
            pie = GetComponent<ProjectileImpactExplosion>();
            if (!pie || !pie.timerAfterImpact)
            {
                Destroy(this);
                return;
            }

            beepStopwatch = 0f;
            beepDuration = pie.lifetimeAfterImpact / timesToBeep;
        }

        private void Update()
        {
            if (!NetworkServer.active || !pie.hasImpact || timesBeeped >= timesToBeep) return;
            beepStopwatch -= Time.deltaTime;
            if (beepStopwatch <= 0f)
            {
                //I dont think this fixes the physics weirdness
                gameObject.layer = LayerIndex.projectileWorldOnly.intVal;
                beepStopwatch = beepDuration;
                EffectManager.SimpleEffect(beepEffectPrefab, transform.position, transform.rotation, true);
                timesBeeped++;
            }
        }
    }
}

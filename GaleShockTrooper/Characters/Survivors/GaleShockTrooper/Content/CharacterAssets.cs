using RoR2;
using UnityEngine;
using GaleShockTrooper.Modules;
using System;
using RoR2.Projectile;
using R2API;
using System.IO;
using BepInEx;
using UnityEngine.AddressableAssets;
using EntityStates.GaleShockTrooperStates.Weapon;
using EntityStates.GaleShockTrooperStates.Weapon.MissilePainter;

namespace GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content
{
    public static class CharacterAssets
    {
        private static AssetBundle _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
            LoadSounds();
            CreatePrimaryShotgunTracer();
            CreateSecondaryMissiles();
            CreateSpecialSlugAssets();
        }

        private static void LoadSounds()
        {
            using (Stream manifestResourceStream = new FileStream(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(GaleShockTrooperPlugin.instance.Info.Location), "SoundBanks")
                + "\\GaleShockTrooperSoundbank.bnk", FileMode.Open))
            {
                byte[] array = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }

        private static void CreatePrimaryShotgunTracer()
        {
            GameObject effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoShotgun.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_ShotgunTracer", false);
            effect.GetComponent<Tracer>().speed = 120f;
            Modules.Content.AddEffectDef(new EffectDef(effect));
            EntityStates.GaleShockTrooperStates.Weapon.FireShotgun.tracerEffectPrefab = effect;
        }

        private static void CreateSecondaryMissiles()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/MissileProjectile.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_MicroMissileProjectile", true);
            ProjectileDamage pd = projectile.GetComponent<ProjectileDamage>();
            pd.damageType = DamageTypeCombo.Generic;

            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            pc.procCoefficient = 1f;
            pc.procChainMask = default;

            MissileController mc = projectile.GetComponent<MissileController>();
            mc.maxVelocity = 30f;
            mc.acceleration = 18f;
            mc.turbulence = 0f;

            UnityEngine.Object.Destroy(projectile.GetComponent<ProjectileSingleTargetImpact>());
            ProjectileImpactExplosion pie = projectile.AddComponent<ProjectileImpactExplosion>();
            pie.blastProcCoefficient = 1f;
            pie.blastAttackerFiltering = AttackerFiltering.Default;
            pie.blastDamageCoefficient = 1f;
            pie.blastRadius = 3f;
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = true;
            pie.lifetime = 20f;
            pie.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            pie.falloffModel = BlastAttack.FalloffModel.None;

            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            FireMissiles.projectilePrefab = projectile;
        }

        private static void CreateSpecialSlugAssets()
        {
            GameObject impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/ImpactRailgun.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_SlugTracerEffect", false);
            EffectComponent ec = impactEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_bandit_M2_shot";
            ContentAddition.AddEffect(impactEffect);
            FireRicochetSlug.ricochetImpactEffect = impactEffect;

            GameObject tracerEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/TracerBandit2Rifle.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_SlugTracerEffect", false);
            var lr = tracerEffect.GetComponentInChildren<LineRenderer>();
            lr.startWidth = 0.4f;
            lr.endWidth = 0.4f;
            lr.widthMultiplier = 0.4f;
            var tracer = tracerEffect.GetComponent<Tracer>();
            ContentAddition.AddEffect(tracerEffect);
            FireRicochetSlug.tracerEffectPrefab = tracerEffect;

            Color orbColor = new Color32(255, 100, 215, 255);
            GameObject orbEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/ChainGunOrbEffect.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_SlugOrbEffect", false);
            var tr = orbEffect.transform.Find("TrailParent/Trail").GetComponent<TrailRenderer>();
            tr.startColor = orbColor;
            tr.endColor = orbColor;
            tr.startWidth = 0.3f;
            tr.endWidth = 0.3f;
            tr.widthMultiplier = 0.3f;
            ContentAddition.AddEffect(orbEffect);
            FireRicochetSlug.orbEffectPrefab = orbEffect;
        }
    }
}

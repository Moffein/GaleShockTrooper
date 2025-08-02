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
using GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Components;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content
{
    public static class CharacterAssets
    {
        private static AssetBundle _assetBundle;
        public static GameObject bossMusicControllerPrefab;

        public static void Init(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
            LoadSounds();
            CreatePrimaryShotgunTracer();
            CreateSecondaryMissiles();
            CreateMiniSmokeRing();
            CreateSecondarySticky();
            CreateSpecialSlugAssets();
            CreateMusicController();
        }

        private static void CreateMusicController()
        {
            GameObject musicControllerObject = GaleShockTrooperSurvivor.instance.assetBundle.LoadAsset<GameObject>("EmptyGameobject").InstantiateClone("GaleShockTrooper_MusicController", false);
            musicControllerObject.AddComponent<NetworkIdentity>();
            var musicController = musicControllerObject.AddComponent<BossMusicController>();
            musicController.soundName = "Play_GaleShockTrooper_Music_Start";
            ContentPacks.networkedObjectPrefabs.Add(musicControllerObject);
            bossMusicControllerPrefab = musicControllerObject;
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
            GameObject effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoDefault.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_ShotgunTracer", false);
            //effect.GetComponent<Tracer>().speed = 250f;

            LineRenderer[] lrs = effect.GetComponentsInChildren<LineRenderer>();
            for (int i = 0; i < lrs.Length; i++)
            {
                //if (!lrs[i].sharedMaterial.ToString().Contains("matCommandoShotgunTracerCore")) UnityEngine.Object.Destroy(lrs[i]);

                /*float blue = lrs[i].startColor.b;
                float red = lrs[i].startColor.r;
                lrs[i].startColor = new Color(blue, lrs[i].startColor.g, red);

                blue = lrs[i].endColor.b;
                red = lrs[i].endColor.r;
                lrs[i].endColor = new Color(blue, lrs[i].endColor.g, red);*/

                //lrs[i].startWidth = 0.15f;
                //lrs[i].endWidth = 0.15f;
                lrs[i].startColor = new Color(0.5f, 0.5f, 1f, lrs[i].startColor.a);
                lrs[i].endColor = new Color(0.5f, 0.5f, 1f, lrs[i].endColor.a);
            }

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

        private static void CreateSecondarySticky()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoGrenadeProjectile.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_StickyProjectile", true);

            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.lifetime = 30f;

            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            TeamComponent tc = projectile.AddComponent<TeamComponent>();
            tc.hideAllyCardDisplay = true;

            ProjectileImpactExplosion pie = projectile.GetComponent<ProjectileImpactExplosion>();
            pie.lifetimeAfterImpact = ThrowSticky.detonationDelay;
            pie.timerAfterImpact = true;
            pie.blastRadius = ThrowSticky.blastRadius;
            pie.destroyOnDistance = false;
            pie.destroyOnWorld = false;
            pie.destroyOnEnemy = false;
            pie.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Scav/ScavSackExplosion.prefab").WaitForCompletion();
            pie.falloffModel = BlastAttack.FalloffModel.None;

            SkillLocator sk = projectile.AddComponent<SkillLocator>();

            CharacterBody cb = projectile.AddComponent<CharacterBody>();
            cb.bodyFlags = CharacterBody.BodyFlags.Masterless;
            cb.baseMaxHealth = 35f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseNameToken = "UNIDENTIFIED";
            cb.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Uninitialized));

            HealthComponent hc = projectile.AddComponent<HealthComponent>();
            hc.dontShowHealthbar = true;

            projectile.AddComponent<ProjectileGrantOnKillOnDestroy>();
            projectile.AddComponent<AssignTeamFilterToTeamComponent>();

            var pbs = projectile.AddComponent<ProjectileBeepAfterStick>();
            pbs.timesToBeep = Mathf.FloorToInt(ThrowSticky.detonationDelay / 0.5f);
            pbs.beepEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_StickyBeep", false);
            EffectComponent ec = pbs.beepEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_commando_M2_grenade_beep";
            Modules.ContentPacks.effectDefs.Add(new EffectDef(pbs.beepEffectPrefab));

            ProjectileStickOnImpact psi = projectile.AddComponent<ProjectileStickOnImpact>();
            psi.ignoreCharacters = false;
            psi.ignoreWorld = false;

            ContentPacks.bodyPrefabs.Add(projectile);
            ContentPacks.projectilePrefabs.Add(projectile);
            ThrowSticky.projectilePrefab = projectile;
        }

        private static void CreateMiniSmokeRing()
        {
            GameObject effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_MiniSmokeRing", false);
            ParticleSystemRenderer[] renderers = effect.GetComponentsInChildren<ParticleSystemRenderer>();
            foreach (var psr in renderers)
            {
                psr.maxParticleSize *= 0.4f;
                psr.transform.localScale *= 0.4f;
            }

            Modules.ContentPacks.effectDefs.Add(new EffectDef(effect));
            PaintMissiles.smokeEffectPrefab = effect;
        }

        private static void CreateSpecialSlugAssets()
        {
            GameObject impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXFMJ.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_SlugTracerEffect", false);
            EffectComponent ec = impactEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_bandit_M2_shot";
            Modules.ContentPacks.effectDefs.Add(new EffectDef(impactEffect));
            FireRicochetSlug.ricochetImpactEffect = impactEffect;

            GameObject tracerEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoShotgun.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_SlugTracerEffect", false);
            /*var lr = tracerEffect.GetComponentInChildren<LineRenderer>();
            lr.startWidth = 0.4f;
            lr.endWidth = 0.4f;
            lr.widthMultiplier = 0.4f;*/
            var tracer = tracerEffect.GetComponent<Tracer>();
            tracer.speed = 150f;

            Color orbColor = new Color32(50, 150, 255, 255);

            Modules.ContentPacks.effectDefs.Add(new EffectDef(tracerEffect));
            FireRicochetSlug.tracerEffectPrefab = tracerEffect;

            GameObject orbEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/ChainGunOrbEffect.prefab").WaitForCompletion().InstantiateClone("GaleShockTrooper_SlugOrbEffect", false);
            var tr = orbEffect.transform.Find("TrailParent/Trail").GetComponent<TrailRenderer>();
            tr.startColor = orbColor;
            tr.endColor = orbColor;
            tr.startWidth = 0.3f;
            tr.endWidth = 0.3f;
            tr.widthMultiplier = 0.3f;
            Modules.ContentPacks.effectDefs.Add(new EffectDef(orbEffect));
            FireRicochetSlug.orbEffectPrefab = orbEffect;
        }
    }
}

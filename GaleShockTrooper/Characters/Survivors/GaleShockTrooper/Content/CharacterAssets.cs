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
            effect.GetComponent<Tracer>().speed = 360f;
            Modules.Content.AddEffectDef(new EffectDef(effect));
            EntityStates.GaleShockTrooperStates.Weapon.FireShotgun.tracerEffectPrefab = effect;
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
            tr.startWidth = 0.4f;
            tr.endWidth = 0.4f;
            tr.widthMultiplier = 0.4f;
            ContentAddition.AddEffect(orbEffect);
            FireRicochetSlug.orbEffectPrefab = orbEffect;
        }
    }
}

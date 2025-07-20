using RoR2;
using UnityEngine;
using GaleShockTrooper.Modules;
using System;
using RoR2.Projectile;
using R2API;
using System.IO;
using BepInEx;
using UnityEngine.AddressableAssets;

namespace GaleShockTrooper.Survivors.GaleShockTrooper.Content
{
    public static class CharacterAssets
    {
        private static AssetBundle _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
            LoadSounds();
            CreatePrimaryShotgunTracer();
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
            EntityStates.GaleShockTrooper.Weapon.FireShotgun.tracerEffectPrefab = effect;
        }
    }
}

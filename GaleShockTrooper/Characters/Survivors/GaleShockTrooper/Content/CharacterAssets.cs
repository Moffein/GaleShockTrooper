using RoR2;
using UnityEngine;
using GaleShockTrooper.Modules;
using System;
using RoR2.Projectile;
using R2API;
using System.IO;
using BepInEx;

namespace GaleShockTrooper.Survivors.GaleShockTrooper.Content
{
    public static class CharacterAssets
    {
        private static AssetBundle _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
            LoadSounds();
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
    }
}

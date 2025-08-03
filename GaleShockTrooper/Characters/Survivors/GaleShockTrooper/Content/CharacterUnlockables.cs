using GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Achievements;
using GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Achievements;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content
{
    public static class CharacterUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            characterUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                UnlockAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(UnlockAchievement.identifier),
                GaleShockTrooperSurvivor.instance.assetBundle.LoadAsset<Sprite>("texGaleShockTrooperPortrait"));

            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                MasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(MasteryAchievement.identifier),
                GaleShockTrooperSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}

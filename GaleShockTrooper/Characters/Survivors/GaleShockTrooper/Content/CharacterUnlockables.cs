using GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Achievements;
using RoR2;
using UnityEngine;

namespace GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content
{
    public static class CharacterUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                MasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(MasteryAchievement.identifier),
                GaleShockTrooperSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}

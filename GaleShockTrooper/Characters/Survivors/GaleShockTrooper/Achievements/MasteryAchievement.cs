using RoR2;
using GaleShockTrooper.Modules.Achievements;

namespace GaleShockTrooper.Survivors.GaleShockTrooper.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class MasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = GaleShockTrooperSurvivor.TOKEN_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = GaleShockTrooperSurvivor.TOKEN_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => GaleShockTrooperSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}
using GaleShockTrooper.Modules.Achievements;
using GaleShockTrooper.Survivors.GaleShockTrooperSurvivor;
using RoR2;
using RoR2.Achievements;
using System.Linq;

namespace GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 3, null)]
    public class DroneAchievement : BaseEndingAchievement
    {
        public const string identifier = GaleShockTrooperSurvivor.TOKEN_PREFIX + "droneAchievement";
        public const string unlockableIdentifier = GaleShockTrooperSurvivor.TOKEN_PREFIX + "droneUnlockable";

        public override bool ShouldGrant(RunReport runReport)
        {
            var playerInfo = runReport.FindPlayerInfo(LocalUserManager.GetFirstLocalUser());
            if (playerInfo != null && playerInfo.bodyIndex == GaleShockTrooperSurvivor.bodyIndex)
            {
                if (playerInfo.equipment.Contains(RoR2Content.Equipment.QuestVolatileBattery.equipmentIndex)) return true;
            }

            return false;
        }
    }
}

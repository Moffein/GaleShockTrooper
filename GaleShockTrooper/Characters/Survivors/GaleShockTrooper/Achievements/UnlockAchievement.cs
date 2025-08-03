using GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Bossfight;
using GaleShockTrooper.Survivors.GaleShockTrooperSurvivor;
using GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content;
using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 3u, typeof(UnlockServerAchievement))]
    public class UnlockAchievement : BaseAchievement
    {
        public const string identifier = GaleShockTrooperSurvivor.TOKEN_PREFIX + "unlockAchievement";
        public const string unlockableIdentifier = "Characters.GaleShockTrooper";
        public override void OnInstall()
        {
            base.OnInstall();
            base.SetServerTracked(true);
        }

        public override void OnUninstall()
        {
            base.OnUninstall();
        }

        private class UnlockServerAchievement : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                BossMissionController.onBossDefeatedGlobal += BossMissionController_onBossDefeatedGlobal;
                if (!CharacterConfig.alwaysTriggerBossfight)
                {
                    Stage.onServerStageBegin += BossMissionController.Stage_onServerStageBegin;
                }
            }

            private void BossMissionController_onBossDefeatedGlobal(BossMissionController obj)
            {
                base.Grant();
            }

            public override void OnUninstall()
            {
                BossMissionController.onBossDefeatedGlobal -= BossMissionController_onBossDefeatedGlobal;
                if (!CharacterConfig.alwaysTriggerBossfight)
                {
                    Stage.onServerStageBegin -= BossMissionController.Stage_onServerStageBegin;
                }
                base.OnUninstall();
            }
        }
    }
}

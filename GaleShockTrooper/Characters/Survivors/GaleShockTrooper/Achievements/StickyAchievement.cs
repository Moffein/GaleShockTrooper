using GaleShockTrooper.Survivors.GaleShockTrooperSurvivor;
using RoR2;
using RoR2.Achievements;
using UnityEngine;
using UnityEngine.Networking;


namespace GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 3u, typeof(StickyServerAchievement))]
    public class StickyAchievement : BaseAchievement
    {
        public static int killsToUnlock = 200;
        public const string identifier = GaleShockTrooperSurvivor.TOKEN_PREFIX + "stickyAchievement";
        public const string unlockableIdentifier = GaleShockTrooperSurvivor.TOKEN_PREFIX + "stickyUnlockable";
        public override void OnInstall()
        {
            base.OnInstall();
            base.SetServerTracked(true);
        }

        public override void OnUninstall()
        {
            base.OnUninstall();
        }

        private class StickyServerAchievement : BaseServerAchievement
        {
            public int killedServer = 0;
            public override void OnInstall()
            {
                base.OnInstall();
                killedServer = 0;
                //RoR2.Run.onRunStartGlobal += Run_onRunStartGlobal;
                On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            }

            private void Run_onRunStartGlobal(Run obj)
            {
                killedServer = 0;
            }

            private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
            {
                orig(self, damageReport);
                if (NetworkServer.active && damageReport.attackerBody && damageReport.attackerBody.bodyIndex == GaleShockTrooperSurvivor.bodyIndex && damageReport.damageInfo.damageType.damageType.HasFlag(DamageType.AOE))
                {
                    killedServer++;
                    Debug.Log("Total: " + killedServer);
                    if (killedServer >= killsToUnlock)
                    {
                        base.Grant();
                    }
                }
            }

            public override void OnUninstall()
            {
                //RoR2.Run.onRunStartGlobal -= Run_onRunStartGlobal;
                On.RoR2.GlobalEventManager.OnCharacterDeath -= GlobalEventManager_OnCharacterDeath;
                base.OnUninstall();
            }
        }
    }
}

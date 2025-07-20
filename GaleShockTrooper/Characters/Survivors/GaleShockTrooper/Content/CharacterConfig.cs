using BepInEx.Configuration;
using GaleShockTrooper.Modules;

namespace GaleShockTrooper.Survivors.GaleShockTrooper.Content
{
    public static class CharacterConfig
    {
        public static ConfigEntry<bool> forceUnlock;

        public static void Init()
        {
            string section = "Shock Trooper";

            forceUnlock = Modules.Config.BindAndOptions(
                section,
                "Force Unlock",
                false,
                "TODO: IMPLEMENT UNLOCK CONDITION.");
        }
    }
}

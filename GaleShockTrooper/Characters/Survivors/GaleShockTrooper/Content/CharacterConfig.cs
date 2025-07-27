using BepInEx.Configuration;
using EntityStates.GaleShockTrooperStates.Dash;
using EntityStates.GaleShockTrooperStates.Weapon;
using EntityStates.GaleShockTrooperStates.Weapon.MissilePainter;
using EntityStates.Jellyfish;
using GaleShockTrooper.Modules;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content
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

            GaleShockTrooperSurvivor.passiveFrontArmorMult = 1f - Modules.Config.BindAndOptions<float>("Skills - Passive Front Armor", "Damage Reduction", 1f/3f, "How much damage this passive reduces.", true).Value;

            FireShotgun.damageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Auto Shotgun", "Damage Coefficient", 0.6f, "How much damage this skill does.", true).Value;
            FireShotgun.pelletCount = Modules.Config.BindAndOptions<uint>("Skills - Auto Shotgun", "Pellet Count", 5, "Pellets per shot.", true).Value;
            FireShotgun.procCoefficient = Modules.Config.BindAndOptions<float>("Skills - Auto Shotgun", "Proc Coefficient", 0.6f, "Affects chance and power of procs.", true).Value;
            FireShotgun.baseDuration = Modules.Config.BindAndOptions<float>("Skills - Auto Shotgun", "Base Duration", 0.3f, "How long it takes to fire this skill.", true).Value;

            FireMissiles.damageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Damage Coefficient", 4f, "How much damage this skill does.", true).Value;
            FireMissiles.baseDuration = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Base Duration", 0.12f, "How long it takes to fire this skill.", true).Value;
            PaintMissiles.baseLockonAngle = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Lockon Angle", 45f, "View Angle for locking on to targets.", true).Value;
            PaintMissiles.baseLockonDuration = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Lockon Duration", 0.4f, "How long it takes to lock on to a target.", true).Value;
            PaintMissiles.baseLockonRange = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Lockon Range", 200f, "Max lockon distance.", true).Value;
            PaintMissiles.baseMaxStocks = Modules.Config.BindAndOptions<int>("Skills - Micro Missiles", "Stocks", 3, "How many charges this skill starts with.", true).Value;
            PaintMissiles.baseCooldown = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Cooldown", 2.5f, "How long it takes for each stock to recharge.", true).Value;
            PaintMissiles.selectedInput = Modules.Config.BindAndOptions<PaintMissiles.InputMode>("Skills - Micro Missiles", "Input Mode", PaintMissiles.InputMode.Hold, "Input Mode for Micro Missiles", true);

            EnterShockDash.baseDuration = Modules.Config.BindAndOptions<float>("Skills - Dash", "Windup Duration", 0.2f, "Delay before entering the main dash.", true).Value;
            ShockDashBase.baseDuration = Modules.Config.BindAndOptions<float>("Skills - Dash", "Base Duration", 0.3f, "How long it take to use this skill.", true).Value;
            ShockDashBase.baseSpeed = Modules.Config.BindAndOptions<float>("Skills - Dash", "Speed", 8f, "How fast the dash travels.", true).Value;
            ShockDashBase.shockDamageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Dash", "Damage Coefficient", 1f, "How much damage this skill deals.", true).Value;
            ShockDashBase.shockRange = Modules.Config.BindAndOptions<float>("Skills - Dash", "Shock Range", 12f, "Range of shocking lightning.", true).Value;
            EnterShockDash.baseCooldown = Modules.Config.BindAndOptions<float>("Skills - Dash", "Cooldown", 5f, "How long it takes for this skill to recharge.", true).Value;

            FireRicochetSlug.damageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Ricochet Slug", "Damage Coefficient", 10f, "How much damage this skill deals.", true).Value;
            FireRicochetSlug.baseCooldown = Modules.Config.BindAndOptions<float>("Skills - Ricochet Slug", "Cooldown", 10f, "How long it takes for this skill to recharge.", true).Value;
            FireRicochetSlug.ricochetCount = Modules.Config.BindAndOptions<int>("Skills - Ricochet Slug", "Ricochet Count", 9, "How many extra targets this can ricochet to.", true).Value;
            FireRicochetSlug.ricochetRange = Modules.Config.BindAndOptions<float>("Skills - Ricochet Slug", "Ricochet Range", 45f, "How far this can ricochet to.", true).Value;

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions")) RiskOfOptionsCompat();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RiskOfOptionsCompat()
        {
            RiskOfOptions.ModSettingsManager.SetModIcon(GaleShockTrooperSurvivor.instance.assetBundle.LoadAsset<Sprite>("texGaleShockTrooperPortrait"));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.ChoiceOption(PaintMissiles.selectedInput));
        }
    }
}

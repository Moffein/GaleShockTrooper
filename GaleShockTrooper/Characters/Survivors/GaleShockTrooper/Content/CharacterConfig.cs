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

            FireShotgun.damageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Auto Shotgun", "Damage Coefficient", FireShotgun.damageCoefficient, "How much damage this skill does.", true).Value;
            FireShotgun.pelletCount = Modules.Config.BindAndOptions<uint>("Skills - Auto Shotgun", "Pellet Count", FireShotgun.pelletCount, "Pellets per shot.", true).Value;
            FireShotgun.procCoefficient = Modules.Config.BindAndOptions<float>("Skills - Auto Shotgun", "Proc Coefficient", FireShotgun.procCoefficient, "Affects chance and power of procs.", true).Value;
            FireShotgun.baseDuration = Modules.Config.BindAndOptions<float>("Skills - Auto Shotgun", "Base Duration", FireShotgun.baseDuration, "How long it takes to fire this skill.", true).Value;

            FireMissiles.damageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Damage Coefficient", FireMissiles.damageCoefficient, "How much damage this skill does.", true).Value;
            FireMissiles.baseDuration = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Base Duration", FireMissiles.baseDuration, "How long it takes to fire this skill.", true).Value;
            PaintMissiles.baseLockonAngle = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Lockon Angle", PaintMissiles.baseLockonAngle, "View Angle for locking on to targets.", true).Value;
            PaintMissiles.baseLockonDuration = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Lockon Duration", PaintMissiles.baseLockonDuration, "How long it takes to lock on to a target.", true).Value;
            PaintMissiles.baseLockonRange = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Lockon Range", PaintMissiles.baseLockonRange, "Max lockon distance.", true).Value;
            PaintMissiles.baseMaxStocks = Modules.Config.BindAndOptions<int>("Skills - Micro Missiles", "Stocks", PaintMissiles.baseMaxStocks, "How many charges this skill starts with.", true).Value;
            PaintMissiles.baseCooldown = Modules.Config.BindAndOptions<float>("Skills - Micro Missiles", "Cooldown", PaintMissiles.baseCooldown, "How long it takes for each stock to recharge.", true).Value;
            PaintMissiles.selectedInput = Modules.Config.BindAndOptions<PaintMissiles.InputMode>("Skills - Micro Missiles", "Input Mode", PaintMissiles.InputMode.Hold, "Input Mode for Micro Missiles", true);

            EnterShockDash.baseDuration = Modules.Config.BindAndOptions<float>("Skills - Dash", "Windup Duration", EnterShockDash.baseDuration, "Delay before entering the main dash.", true).Value;
            ShockDashBase.baseDuration = Modules.Config.BindAndOptions<float>("Skills - Dash", "Base Duration", ShockDashBase.baseDuration, "How long it take to use this skill.", true).Value;
            ShockDashBase.baseSpeed = Modules.Config.BindAndOptions<float>("Skills - Dash", "Speed", ShockDashBase.baseSpeed, "How fast the dash travels.", true).Value;
            ShockDashBase.shockDamageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Dash", "Damage Coefficient", ShockDashBase.shockDamageCoefficient, "How much damage this skill deals.", true).Value;
            ShockDashBase.shockRange = Modules.Config.BindAndOptions<float>("Skills - Dash", "Shock Range", ShockDashBase.shockRange, "Range of shocking lightning.", true).Value;
            EnterShockDash.baseCooldown = Modules.Config.BindAndOptions<float>("Skills - Dash", "Cooldown", EnterShockDash.baseCooldown, "How long it takes for this skill to recharge.", true).Value;

            FireRicochetSlug.damageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Ricochet Slug", "Damage Coefficient", FireRicochetSlug.damageCoefficient, "How much damage this skill deals.", true).Value;
            FireRicochetSlug.baseCooldown = Modules.Config.BindAndOptions<float>("Skills - Ricochet Slug", "Cooldown", FireRicochetSlug.baseCooldown, "How long it takes for this skill to recharge.", true).Value;
            FireRicochetSlug.ricochetCount = Modules.Config.BindAndOptions<int>("Skills - Ricochet Slug", "Ricochet Count", FireRicochetSlug.ricochetCount, "How many extra targets this can ricochet to.", true).Value;
            FireRicochetSlug.ricochetRange = Modules.Config.BindAndOptions<float>("Skills - Ricochet Slug", "Ricochet Range", FireRicochetSlug.ricochetRange, "How far this can ricochet to.", true).Value;

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

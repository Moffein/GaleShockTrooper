using BepInEx.Configuration;
using EntityStates.GaleShockTrooperDroneStates;
using EntityStates.GaleShockTrooperStates.Dash;
using EntityStates.GaleShockTrooperStates.Weapon;
using EntityStates.GaleShockTrooperStates.Weapon.MissilePainter;
using EntityStates.Jellyfish;
using GaleShockTrooper.Characters.Drones.GaleShockTrooperDrone;
using GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Components;
using GaleShockTrooper.Modules;
using R2API.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content
{
    public static class CharacterConfig
    {
        public static bool forceUnlock;
        public static bool alwaysTriggerBossfight;

        public static void Init()
        {
            string section = "Shock Trooper";

            forceUnlock = Modules.Config.BindAndOptions(
                section,
                "Force Unlock",
                false,
                "Unlock the survivor and all skills.").Value;
            alwaysTriggerBossfight = Modules.Config.BindAndOptions<bool>(section, "Always Trigger Bossfight", false, "Trigger the bossfight even after unlocking the survivor", true).Value;

            bool useStatConfig = Modules.Config.BindAndOptions<bool>(section, "Stat Config", false, "Enables config editing for skill numbers.", true).Value;
            PaintMissiles.selectedInput = Modules.Config.BindAndOptions<PaintMissiles.InputMode>("Inputs", "Micro Missiles - Input Mode", PaintMissiles.InputMode.Hold, "Input Mode for Micro Missiles", false);

            if (useStatConfig)
            {
                GaleShockTrooperSurvivor.passiveFrontArmorMult = 1f - Modules.Config.BindAndOptions<float>("Skills - Passive Front Armor", "Damage Reduction", 1f / 3f, "How much damage this passive reduces.", true).Value;

                GaleShockTrooperSurvivor.baseHealth = Modules.Config.BindAndOptions<float>("Stats", "Base Health", GaleShockTrooperSurvivor.baseHealth, "Starting health, level health is automatically scaled to this.", true).Value;
                GaleShockTrooperSurvivor.baseArmor = Modules.Config.BindAndOptions<float>("Stats", "Base Armor", GaleShockTrooperSurvivor.baseArmor, "Starting armor.", true).Value;

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

                ThrowSticky.damageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Stickybomb", "Damage Coefficient", ThrowSticky.damageCoefficient, "How much damage this skill does.", true).Value;
                ThrowSticky.baseCooldown = Modules.Config.BindAndOptions<float>("Skills - Stickybomb", "Cooldown", ThrowSticky.baseCooldown, "How long it takes for this skill to recharge.", true).Value;
                ThrowSticky.blastRadius = Modules.Config.BindAndOptions<float>("Skills - Stickybomb", "Blast Radius", ThrowSticky.blastRadius, "How big is the explosion.", true).Value;
                ThrowSticky.baseMaxStocks = Modules.Config.BindAndOptions<int>("Skills - Stickybomb", "Stocks", ThrowSticky.baseMaxStocks, "How many charges this skill starts with.", true).Value;
                ThrowSticky.detonationDelay = Modules.Config.BindAndOptions<float>("Skills - Stickybomb", "Detonation Delay", ThrowSticky.detonationDelay, "Delay before explosion.", true).Value;
                ThrowSticky.projectileSpeed = Modules.Config.BindAndOptions<float>("Skills - Stickybomb", "Projectile Speed", ThrowSticky.projectileSpeed, "How fast the projectile travels.", true).Value;

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
                FireRicochetSlug.selfKnockbackForce = Modules.Config.BindAndOptions<float>("Skills - Ricochet Slug", "Self Knockback Force", FireRicochetSlug.selfKnockbackForce, "Self knockback force when firing this skill.", true).Value;

                FireAutoTurret.damageCoefficient = Modules.Config.BindAndOptions<float>("Skills - Drone Attack", "Damage Coefficient", FireAutoTurret.damageCoefficient, "How much damage this skill deals.", true).Value;
                FireAutoTurret.baseDuration = Modules.Config.BindAndOptions<float>("Skills - Drone Attack", "Base Duration", FireAutoTurret.baseDuration, "How long it take to use this skill.", true).Value;
                FireAutoTurret.baseShotDuration = Modules.Config.BindAndOptions<float>("Skills - Drone Attack", "Base Shot Duration", FireAutoTurret.baseShotDuration, "How long it takes to fire a shot in a burst.", true).Value;
                FireAutoTurret.shotsPerBurst = Modules.Config.BindAndOptions<int>("Skills - Drone Attack", "Shots Per burst", FireAutoTurret.shotsPerBurst, "How long it takes to fire a shot in a burst.", true).Value;
                MasterDroneTracker.baseMaxDrones = Modules.Config.BindAndOptions<int>("Skills - Deploy Drone", "Stocks", MasterDroneTracker.baseMaxDrones, "How many charges this skill starts with. Max drones is based on this.", true).Value;
                MasterDroneTracker.maxExtraDrones = Modules.Config.BindAndOptions<int>("Skills - Deploy Drone", "Max Extra Drones", MasterDroneTracker.maxExtraDrones, "How many extra drones you can get with stock-increasing items.", true).Value;
                DeployDrone.baseCooldown = Modules.Config.BindAndOptions<float>("Skills - Deploy Drone", "Cooldown", DeployDrone.baseCooldown, "How long it takes for this skill to recharge.", true).Value;
                GaleShockTrooperDroneCharacter.baseHealth = Modules.Config.BindAndOptions<float>("Skills - Deploy Drone", "Drone Base Health", GaleShockTrooperDroneCharacter.baseHealth, "Starting health, level health is automatically scaled to this.", true).Value;
                GaleShockTrooperDroneCharacter.baseArmor = Modules.Config.BindAndOptions<float>("Skills - Deploy Drone", "Drone Base Armor", GaleShockTrooperDroneCharacter.baseArmor, "Starting armor.", true).Value;
            }

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

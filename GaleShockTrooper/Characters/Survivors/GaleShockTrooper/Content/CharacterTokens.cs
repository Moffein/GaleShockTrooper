using System;
using EntityStates.GaleShockTrooperDroneStates;
using EntityStates.GaleShockTrooperStates.Dash;
using EntityStates.GaleShockTrooperStates.Weapon;
using EntityStates.GaleShockTrooperStates.Weapon.MissilePainter;
using GaleShockTrooper.Characters.Survivors.GaleShockTrooper.Achievements;
using GaleShockTrooper.Modules;
using GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Achievements;
using UnityEngine;

namespace GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Content
{
    public static class CharacterTokens
    {
        public static void Init()
        {
            AddHenryTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddHenryTokens()
        {
            string prefix = GaleShockTrooperSurvivor.TOKEN_PREFIX;

            string desc = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            Language.Add(prefix + "NAME", "Shock Trooper");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Interstellar Bounty Hunter");
            Language.Add(prefix + "LORE", "A lone skorg sits against a rock, gazing up at the night sky.  The grass ripples like water from a gentle breeze.  A human approaches and sits next to him.\n\n“Never thought of your species as the meditative kind,” spoke the human.\n\n“Earth is much more temperate than Sabbides,” the skorg responded.\n\nThe human nods, aware of the muggy, volcanic jungles the skorg originates from.  A few minutes pass as the two continue to stargaze.  The human then speaks again.\n\n“You’re thinking of going, aren’t you?”\n\nThe skorg nods.  The human speaks again.\n\n“You really shouldn’t.  I’ve read the same reports you have.  It’s a shitshow down there.“\n\nThe skorg takes a deep breath.  Then he sighs.\n\n“A bounty this big could save the resistance.  We could finally turn the tide on this stupid war my people are fighting.”\n\nThe human shakes his head in disappointment, but understands from experience.  He stands, changing the subject to a lighter tone.\n\n“Regardless, you should hurry to the mess hall with me.  I know you’re not a fan of lab grown meat, but you’ve gotta try the printed bighorn steaks.”\n\nThe human receives no response.  He turns and leaves without any further words.  A moment passes, then the skorg stands, his yellow-eyed gaze fixed on stars only he can see.  He closes his eyes, envisioning the stars awash with pink.  A tailwind blows across his spiracles, carrying a damp, earthy scent from the symbiotic moss on his scales.  With a sigh, he opens his eyes again and speaks a single word.\n\n“P e t r i c h o r .”");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Bloodthirst");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Unstoppable Force");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "The Shock Trooper takes <style=cIsUtility>"+Mathf.FloorToInt((1f-GaleShockTrooperSurvivor.passiveFrontArmorMult) * 100f)+"% reduced damage</style> against <style=cIsDamage>projectiles</style>, <style=cIsDamage>blasts</style>, and <style=cIsDamage>bullets</style> from the <style=cIsUtility>front</style>.");
            #endregion

            Language.Add(prefix + "PRIMARY_NAME", "PRD-12 Auto Shotgun");
            Language.Add(prefix + "PRIMARY_DESCRIPTION", "Fire a shotgun blast for <style=cIsDamage>"+FireShotgun.pelletCount+"x"+ Mathf.RoundToInt(FireShotgun.damageCoefficient * 100f) + "%</style> damage.");

            Language.Add(prefix + "SECONDARY_NAME", "Micro Missiles");
            Language.Add(prefix + "SECONDARY_DESCRIPTION", "Enter <style=cIsUtility>target painting mode</style> to launch heat-seeking missiles that deal <style=cIsDamage>" + Mathf.RoundToInt(FireMissiles.damageCoefficient * 100f) + "% damage</style> each. Can store up to "+PaintMissiles.baseMaxStocks+".");

            Language.Add(prefix + "SECONDARY_STICKY_NAME", "Thqwib Bomb");
            string stickyString = "Throw a Thqwib-mounted bomb that explodes for <style=cIsDamage>" + Mathf.RoundToInt(ThrowSticky.damageCoefficient * 100f) + "% damage</style> and <style=cIsDamage>triggers on-kill effects</style>.";
            if (ThrowSticky.baseMaxStocks > 1) stickyString += " Hold up to " + ThrowSticky.baseMaxStocks + ".";
            Language.Add(prefix + "SECONDARY_STICKY_DESCRIPTION", stickyString);

            Language.Add(prefix + "UTILITY_NAME", "Overdrive");
            Language.Add(prefix + "UTILITY_DESCRIPTION", "<style=cIsDamage>Shocking</style>. <style=cIsUtility>Dash</style> a short distance while electrocuting nearby enemies for <style=cIsDamage>"+ Mathf.RoundToInt(ShockDashBase.shockDamageCoefficient * 100f) + "% damage</style>.");

            Language.Add(prefix + "SPECIAL_NAME", "Easy Prey");
            Language.Add(prefix + "SPECIAL_DESCRIPTION", "Fire a slug for <style=cIsDamage>"+ Mathf.RoundToInt(FireRicochetSlug.damageCoefficient * 100f) + "% damage</style>. Upon hitting an enemy, <style=cIsDamage>ricochets</style> to up to <style=cIsDamage>"+ FireRicochetSlug.ricochetCount+ "</style> additional targets.");

            Language.Add(prefix + "SPECIAL_DRONE_NAME", "Hunter Killer");
            Language.Add(prefix + "SPECIAL_DRONE_DESCRIPTION", "Deploy a Hunter Drone that attacks enemies for <style=cIsDamage>" + FireAutoTurret.shotsPerBurst + "x" + Mathf.RoundToInt(FireAutoTurret.damageCoefficient * 100f) + " damage</style>.");

            Language.Add("GALE_GALESHOCKTROOPERDRONE_BODY_NAME", "Hunter Drone");

            #region Achievements
            Language.Add(Modules.Tokens.GetAchievementNameToken(UnlockAchievement.identifier), "Dead Meat");
            Language.Add(Modules.Tokens.GetAchievementDescriptionToken(UnlockAchievement.identifier), "Beat a competitor to his bounty.");

            Language.Add(Modules.Tokens.GetAchievementNameToken(MasteryAchievement.identifier), "Shock Trooper: Mastery");
            Language.Add(Modules.Tokens.GetAchievementDescriptionToken(MasteryAchievement.identifier), "As the Shock Trooper, beat the game or obliterate on Monsoon.");

            Language.Add(Modules.Tokens.GetAchievementNameToken(StickyAchievement.identifier), "Shock Trooper: Boom");
            Language.Add(Modules.Tokens.GetAchievementDescriptionToken(StickyAchievement.identifier), "As the Shock Trooper, kill " + StickyAchievement.killsToUnlock + " enemies with explosives in a single run.");

            Language.Add(Modules.Tokens.GetAchievementNameToken(DroneAchievement.identifier), "Shock Trooper: Powered Up");
            Language.Add(Modules.Tokens.GetAchievementDescriptionToken(DroneAchievement.identifier), "As the Shock Trooper, escape the Moon while carrying the Fuel Array.");
            #endregion
        }
    }
}

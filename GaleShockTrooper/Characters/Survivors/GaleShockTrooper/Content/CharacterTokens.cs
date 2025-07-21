using System;
using GaleShockTrooper.Modules;
using GaleShockTrooper.Survivors.GaleShockTrooperSurvivor.Achievements;

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
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Henry passive");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            Language.Add(prefix + "PRIMARY_NAME", "PRD-12 Auto Shotgun");
            Language.Add(prefix + "PRIMARY_DESCRIPTION", "Fire a shotgun blast for <style=cIsDamage>5x60%</style> damage.");

            Language.Add(prefix + "UTILITY_NAME", "Overdrive");
            Language.Add(prefix + "UTILITY_DESCRIPTION", "<style=cIsDamage>Shocking</style>. <style=cIsUtility>Dash</style> a short distance while electrocuting nearby enemies for <style=cIsDamage>100% damage</style>.");

            Language.Add(prefix + "SPECIAL_NAME", "Easy Prey");
            Language.Add(prefix + "SPECIAL_DESCRIPTION", "Fire a slug for <style=cIsDamage>1000% damage</style>. Upon hitting an enemy, <style=cIsDamage>ricochets</style> to up to <style=cIsDamage>9</style> additional targets.");

            #region Achievements
            Language.Add(Modules.Tokens.GetAchievementNameToken(MasteryAchievement.identifier), "Henry: Mastery");
            Language.Add(Modules.Tokens.GetAchievementDescriptionToken(MasteryAchievement.identifier), "As Henry, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}

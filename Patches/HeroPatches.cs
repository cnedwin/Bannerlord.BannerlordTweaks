﻿using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordTweaks.Patches
{
    [HarmonyPatch(typeof(Hero), "AddSkillXp")]
    public class AddSkillXpPatch
    {
        private static FieldInfo? hdFieldInfo = null;

        /*
        private static float GetMultiplier()
        {
            if (BannerlordTweaksSettings.Instance.HeroSkillExperienceOverrideMultiplierEnabled)
                return BannerlordTweaksSettings.Instance.HeroSkillExperienceMultiplier;
            else
                return Math.Max(1, 0.0315769 * Math.Pow(skillLevel, 1.020743));
        }
        */

        static bool Prefix(Hero __instance, SkillObject skill, float xpAmount)
        {
            try
            {

                if (hdFieldInfo == null) GetFieldInfo();

                HeroDeveloper hd = (HeroDeveloper)hdFieldInfo!.GetValue(__instance);

                if (hd != null)
                {
                    if (xpAmount > 0)
                    {
                        if (BannerlordTweaksSettings.Instance is { } settings && settings.HeroSkillExperienceMultiplierEnabled && hd.Hero.IsHumanPlayerCharacter)
                        {
                            float newXpAmount = (int)Math.Ceiling(xpAmount * BannerlordTweaksSettings.Instance.HeroSkillExperienceMultiplier);
                            hd.AddSkillXp(skill, newXpAmount, true, true);
                            //DebugHelpers.DebugMessage("HeroSkillXPPatch: Player: " + hd.Hero.Name+ "\nSkill is: " + skill.Name + "\nXPAmount = " + xpAmount + "\nNewXPAmount = " + newXpAmount);
                        }
                        else if (BannerlordTweaksSettings.Instance is { } settings2 && settings2.CompanionSkillExperienceMultiplierEnabled && !hd.Hero.IsHumanPlayerCharacter &&
                           (hd.Hero.Clan == Hero.MainHero.Clan))
                        {
                            float newXpAmount = (int)Math.Ceiling(xpAmount * BannerlordTweaksSettings.Instance.CompanionSkillExperienceMultiplier);
                            hd.AddSkillXp(skill, newXpAmount, true, true);
                            //DebugHelpers.DebugMessage("HeroSkillXPPatch: Companion: " + hd.Hero.Name + " - Clan: "+ hd.Hero.Clan.Name + " - Skill is: " + skill.Name + " - XPAmount = " + xpAmount + " - NewXPAmount = " + newXpAmount);
                        }
                        else
                            hd.AddSkillXp(skill, xpAmount, true, true);
                    }
                    else
                        hd.AddSkillXp(skill, xpAmount, true, true);
                }
            }
            catch (Exception ex)
            {
                DebugHelpers.ShowError("An exception occurred while trying to apply the hero xp multiplier.", "", ex);
            }
            return false;
        }

        static bool Prepare()
        {
            if (BannerlordTweaksSettings.Instance is { } settings)
            {
                if (settings.HeroSkillExperienceMultiplierEnabled)
                    GetFieldInfo();
                return settings.HeroSkillExperienceMultiplierEnabled;
            }
            else return false;
        }

        private static void GetFieldInfo()
        {
            hdFieldInfo = typeof(Hero).GetField("_heroDeveloper", BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}
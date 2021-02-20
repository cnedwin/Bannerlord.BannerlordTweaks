﻿using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordTweaks
{
    public class DailyTroopExperienceTweak
    {
        public static void Apply(Campaign campaign)
        {
            var obj = new DailyTroopExperienceTweak();
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(obj, (MobileParty mp) => { obj.DailyTick(mp); });
        }

        private void DailyTick(MobileParty party)
        {
            if (party.LeaderHero != null && BannerlordTweaksSettings.Instance is { } settings)
            {
                int count = party.MemberRoster.GetTroopRoster().Count();
                if (party.LeaderHero == Hero.MainHero || settings.DailyTroopExperienceApplyToAllNPC || (settings.DailyTroopExperienceApplyToPlayerClanMembers && party.LeaderHero.Clan == Clan.PlayerClan) )
                {
                    int experienceAmount = ExperienceAmount(party.LeaderHero);
                    if (experienceAmount > 0)
                    {
                        foreach (var troop in party.MemberRoster.GetTroopRoster())
                        {
                            party.MemberRoster.AddXpToTroop(experienceAmount, troop.Character);
                        }

                        if (settings.DisplayMessageDailyExperienceGain)
                        {
                            string troops = count == 1 ? "soldier" : "troops";
                            //Debug
                            //DebugHelpers.DebugMessage($"{party.LeaderHero.Name}'s party granted {experienceAmount} experience to {count} {troops}."));
                            if (party.LeaderHero == Hero.MainHero)
                                InformationManager.DisplayMessage(new InformationMessage($"综合设置已授予 {experienceAmount} 经验 给 {count} {troops}."));
                        }
                    }
                }
            }
        }

        private static int ExperienceAmount(Hero h)
        {
            int leadership = h.GetSkillValue(DefaultSkills.Leadership);
            if (BannerlordTweaksSettings.Instance is { } settings)
            {
                if (leadership >= settings.DailyTroopExperienceRequiredLeadershipLevel)
                    return (int)(settings.LeadershipPercentageForDailyExperienceGain * leadership);
            }
            return 0;
        }
    }
}

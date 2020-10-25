﻿using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace BannerlordTweaks.Patches
{
    [HarmonyPatch(typeof(CraftingVM), "HaveEnergy")]
    public class CraftingVMPatch
    {
        static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }

        static bool Prepare()
        {
            return BannerlordTweaksSettings.Instance.IgnoreCraftingStamina;
        }
    }
}

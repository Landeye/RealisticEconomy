using HarmonyLib;
using TaleWorlds.CampaignSystem.ComponentInterfaces;  // SettlementEconomyModel lives here
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;

namespace RealisticEconomy.Patches
{
    [HarmonyPatch(typeof(DefaultSettlementEconomyModel), "GetTownGoldChange")]
    public static class DisableSettlementIncomePatch
    {
        static bool Prefix(Town town, ref int __result)
        {
            __result = 0;
            return false; // skip the vanilla method entirely
        }
    }
}
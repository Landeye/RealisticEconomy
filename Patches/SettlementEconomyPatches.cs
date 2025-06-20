using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.GameComponents; // DefaultSettlementEconomyModel

namespace RealisticEconomy.Patches
{
    [HarmonyPatch]
    public static class SettlementEconomyPatches
    {
        // 1) Target the method that adds passive gold.
        //    In Bannerlord 1.2.x this is GetTownGoldChange on DefaultSettlementEconomyModel
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(DefaultSettlementEconomyModel),
                nameof(DefaultSettlementEconomyModel.GetTownGoldChange)
            );
        }

        // 2) Prefix: prevent the original from running by returning false.
        //    We can compute our own delta (e.g., zero or tax-based) if we need.
        static bool Prefix(Town town, ref int __result)
        {
            // Example: always zero out vanilla income
            __result = 0;

            // TODO: replace '0' with your tax-on-production calc:
            //   int production = (int)(town.Prosperity * TaxRate);
            //   __result = production;

            return false; // skip original
        }
    }
}
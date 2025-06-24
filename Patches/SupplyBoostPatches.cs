using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.GameComponents;    // new model classes
using TaleWorlds.CampaignSystem.Settlements;       // Village

namespace RealisticEconomy.Patches
{
    // ───────── tuning knobs ─────────
    internal static class SupplyBoostSettings
    {
        public const float VillageOutputMult = 1.5f;   // +50 %
        public const int ExtraPartySlots = 1;      // +1 (usually a caravan)
        public const float CaravanGuardBoost = 0.25f;  // +25 %
    }

    // A) village production ×1.5
    [HarmonyPatch(typeof(DefaultVillageProductionCalculatorModel),
                  nameof(DefaultVillageProductionCalculatorModel.CalculateDailyProductionAmount))]
    internal static class VillageProductionPatch
    {
        [HarmonyPostfix]
        private static void Postfix(ref float __result)
            => __result *= SupplyBoostSettings.VillageOutputMult;
    }

    // B) +1 generic party slot per clan tier
    [HarmonyPatch(typeof(DefaultClanTierModel),
                  nameof(DefaultClanTierModel.GetPartyLimitForTier))]
    internal static class ClanPartyCapPatch
    {
        [HarmonyPostfix]
        private static void Postfix(ref int __result)
            => __result += SupplyBoostSettings.ExtraPartySlots;
    }

    // C) +25 % guards on every newly spawned caravan
    [HarmonyPatch(typeof(CaravanPartyComponent), "CreateCaravanParty")]
    internal static class CaravanGuardPatch
    {
        [HarmonyPostfix]
        private static void Postfix(MobileParty __result)
        {
            if (__result == null) return;

            int extra = (int)(__result.MemberRoster.TotalManCount *
                              SupplyBoostSettings.CaravanGuardBoost);
            if (extra <= 0) return;

            var baseTroop = __result.MemberRoster.GetCharacterAtIndex(0);
            __result.MemberRoster.AddToCounts(baseTroop, extra);
        }
    }
}





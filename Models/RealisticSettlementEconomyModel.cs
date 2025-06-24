using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;          // Town
using TaleWorlds.CampaignSystem.ComponentInterfaces; // SettlementEconomyModel
using TaleWorlds.CampaignSystem.GameComponents;      // DefaultSettlementEconomyModel
using TaleWorlds.Core;                               // ItemCategory, ItemData
using RealisticEconomy.Models;

namespace RealisticEconomy.Models
{
    public sealed class RealisticSettlementEconomyModel : SettlementEconomyModel
    {
        private readonly DefaultSettlementEconomyModel _vanilla =
            new DefaultSettlementEconomyModel();

        //── Config knobs ──────────────────────────────────────────────
        private const float GoldPerProsperity = 7f;   // vanilla target line
        private const float DailyDriftFraction = 0.20f;// 20 % of the gap per day
        private const float ShortageDemandBoost = 1.20f;// +20 % demand if price>1

        private readonly Dictionary<Town, int> _treasuries = new Dictionary<Town, int>();

        //── 1) Gold drift  +  multipliers ────────────────────────────
        public override int GetTownGoldChange(Town town)
        {
            if (!_treasuries.TryGetValue(town, out int treasury))
            {
                treasury = (int)(town.Prosperity * GoldPerProsperity);
                _treasuries[town] = treasury;
            }

            int target = (int)(town.Prosperity * GoldPerProsperity);
            int gap = target - treasury;
            int delta = (int)(gap * DailyDriftFraction);   // ← drift part

            // apply your original scaling knobs
            if (delta > 0)
                delta = (int)(delta * REGlobal.TaxIncomeMultiplier);
            else
                delta = (int)(delta * REGlobal.GarrisonExpenseMultiplier);

            _treasuries[town] = treasury + delta;

            // ─── NEW: debug-log the drift that will be applied today ───
            RealisticEconomy.Models.FileLogger.Log(
                $"[Drift] {town.Name} Δgold {delta:+0;-0}");

            return delta;
        }

        //── 2) Shortage → extra demand (feeds prosperity drop) ───────
        public override float GetDailyDemandForCategory(
            Town town,
            ItemCategory category,
            int extraProsperity = 0)
        {
            float demand = _vanilla.GetDailyDemandForCategory(
                               town, category, extraProsperity);

            float idx = town.GetItemCategoryPriceIndex(category);   // 1.00 = perfect balance
            if (idx > 1.0f)
                demand *= 1f + (idx - 1f);          // 1.05  → +5 %,  1.50 → +50 %

            return demand;
        }

        //── 3) All remaining helpers are pure pass-through ───────────
        public override float GetDemandChangeFromValue(float purchaseValue)
            => _vanilla.GetDemandChangeFromValue(purchaseValue);

        public override (float, float) GetSupplyDemandForCategory(
            Town town,
            ItemCategory category,
            float dailySupply,
            float dailyDemand,
            float oldSupply,
            float oldDemand)
            => _vanilla.GetSupplyDemandForCategory(
                   town, category,
                   dailySupply, dailyDemand,
                   oldSupply, oldDemand);

        public override float GetEstimatedDemandForCategory(
            Town town,
            ItemData itemData,
            ItemCategory category)
            => _vanilla.GetEstimatedDemandForCategory(town, itemData, category);
    }
}
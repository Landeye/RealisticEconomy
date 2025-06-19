using TaleWorlds.CampaignSystem.Settlements;             // Town, Settlement, ItemCategory, ItemData
using TaleWorlds.CampaignSystem.ComponentInterfaces;     // SettlementEconomyModel
using TaleWorlds.Core;                                   // InformationManager, InformationMessage
using TaleWorlds.Library;                                // (only for convenience)

namespace RealisticEconomy.Models
{
    public class RealisticSettlementEconomyModel : SettlementEconomyModel
    {
        // 1) Town gold change – exact signature Bannerlord expects
        public override int GetTownGoldChange(Town town)
        {
            // a) Let the game compute its normal gold-change
            int gold = base.GetTownGoldChange(town);

            // b) DEBUG: pop up a message so you know this override is running
            InformationManager.DisplayMessage(
                new InformationMessage(
                    $"[RealisticEconomy] {town.Name} gold change = {gold}"
                )
            );

            // c) Return the unmodified result
            return gold;
        }

        // 2) How much demand shifts per gold-value
        public override float GetDemandChangeForValue(float value)
            => base.GetDemandChangeForValue(value);

        // 3) Supply-vs-demand tuple for a category
        public override (float supply, float demand) GetSupplyDemandForCategory(
            Town town,
            ItemCategory category,
            float currentValue,
            float maxValue,
            float minValue,
            float baseDemand)
            => base.GetSupplyDemandForCategory(town, category, currentValue, maxValue, minValue, baseDemand);

        // 4) Estimated demand for a specific item category
        public override float GetEstimatedDemandForCategory(
            Town town,
            ItemData item,
            ItemCategory category)
            => base.GetEstimatedDemandForCategory(town, item, category);

        // 5) Daily demand for a category over N days
        public override float GetDailyDemandForCategory(
            Town town,
            ItemCategory category,
            int days)
            => base.GetDailyDemandForCategory(town, category, days);

        // 6) Total income for a settlement
        public override int CalculateIncome(Settlement settlement)
            => base.CalculateIncome(settlement);

        // 7) Total expenses for a settlement
        public override int CalculateExpenses(Settlement settlement)
            => base.CalculateExpenses(settlement);
    }
}
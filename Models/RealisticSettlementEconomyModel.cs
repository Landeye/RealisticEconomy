using TaleWorlds.CampaignSystem.Settlements;          // Town, Settlement, ItemCategory, ItemData
using TaleWorlds.CampaignSystem.ComponentInterfaces;  // SettlementEconomyModel
using TaleWorlds.CampaignSystem.GameComponents;       // DefaultSettlementEconomyModel
using RealisticEconomy.Models;                        // FileLogger
using TaleWorlds.Core;

namespace RealisticEconomy.Models
{
    public class RealisticSettlementEconomyModel : SettlementEconomyModel
    {
        private readonly DefaultSettlementEconomyModel _default = new DefaultSettlementEconomyModel();

        // Clear the CSV header once when this class is first accessed
        static RealisticSettlementEconomyModel() => FileLogger.Initialize();

        // 1) Override the daily gold‐change callback
        public override int GetTownGoldChange(Town town)
        {
            int delta = _default.GetTownGoldChange(town);
            FileLogger.LogTick(town, delta);
            return delta;
        }

        // 2) Forward all other abstract members to the concrete default model
        public override float GetDemandChangeFromValue(float purchaseValue)
            => _default.GetDemandChangeFromValue(purchaseValue);

        public override (float, float) GetSupplyDemandForCategory(
            Town town,
            ItemCategory category,
            float dailySupply,
            float dailyDemand,
            float oldSupply,
            float oldDemand)
            => _default.GetSupplyDemandForCategory(
                   town, category,
                   dailySupply, dailyDemand,
                   oldSupply, oldDemand
               );

        public override float GetEstimatedDemandForCategory(
            Town town,
            ItemData itemData,
            ItemCategory category)
            => _default.GetEstimatedDemandForCategory(town, itemData, category);

        public override float GetDailyDemandForCategory(
            Town town,
            ItemCategory category,
            int extraProsperity = 0)
            => _default.GetDailyDemandForCategory(town, category, extraProsperity);
    }
}
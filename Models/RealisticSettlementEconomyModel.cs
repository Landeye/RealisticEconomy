using TaleWorlds.CampaignSystem.Settlements;         // for Town
using TaleWorlds.CampaignSystem.ComponentInterfaces; // for SettlementEconomyModel
using TaleWorlds.CampaignSystem.GameComponents;      // for DefaultSettlementEconomyModel
using TaleWorlds.Core;

namespace RealisticEconomy.Models
{
    public class RealisticSettlementEconomyModel : SettlementEconomyModel
    {
        private readonly DefaultSettlementEconomyModel _default = new DefaultSettlementEconomyModel();

        public override int GetTownGoldChange(Town town)
        {
            // simply forward to the vanilla logic
            return _default.GetTownGoldChange(town);
        }

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


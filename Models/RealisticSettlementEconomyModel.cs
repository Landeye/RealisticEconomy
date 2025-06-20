using TaleWorlds.CampaignSystem.Settlements;      // for Town, ItemCategory, ItemData
using TaleWorlds.CampaignSystem.ComponentInterfaces;  // for SettlementEconomyModel
using TaleWorlds.CampaignSystem.GameComponents;       // for DefaultSettlementEconomyModel
using TaleWorlds.Core;                                // for InformationManager
using TaleWorlds.Library;                             // for InformationMessage

namespace RealisticEconomy.Models
{
    public class RealisticSettlementEconomyModel : SettlementEconomyModel
    {
        // Instantiate the game's default economy model to delegate calls
        private readonly SettlementEconomyModel _defaultModel = new DefaultSettlementEconomyModel();

        // 1) Town gold change
        public override int GetTownGoldChange(Town town)
        {
            int gold = _defaultModel.GetTownGoldChange(town);

            InformationManager.DisplayMessage(
                new InformationMessage($"[RealEco] Daily gold for {town.Name} = {gold}")
            );

            return gold;
        }

        // 2) Demand shift per purchase value
        public override float GetDemandChangeFromValue(float purchaseValue)
            => _defaultModel.GetDemandChangeFromValue(purchaseValue);

        // 3) Supply vs demand tuple (supply, demand)
        public override (float, float) GetSupplyDemandForCategory(
            Town town,
            ItemCategory category,
            float dailySupply,
            float dailyDemand,
            float oldSupply,
            float oldDemand)
            => _defaultModel.GetSupplyDemandForCategory(
                   town, category,
                   dailySupply, dailyDemand,
                   oldSupply, oldDemand
               );

        // 4) Estimated demand for one item category
        public override float GetEstimatedDemandForCategory(
            Town town,
            ItemData itemData,
            ItemCategory category)
            => _defaultModel.GetEstimatedDemandForCategory(town, itemData, category);

        // 5) Daily aggregate demand over N days
        public override float GetDailyDemandForCategory(
            Town town,
            ItemCategory category,
            int extraProsperity = 0)
            => _defaultModel.GetDailyDemandForCategory(town, category, extraProsperity);
    }
}
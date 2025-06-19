using TaleWorlds.CampaignSystem.Settlements;          // Town, Settlement, ItemCategory, ItemData
using TaleWorlds.CampaignSystem.ComponentInterfaces;  // SettlementEconomyModel
using TaleWorlds.CampaignSystem.GameComponents;       // DefaultSettlementEconomyModel
using TaleWorlds.Core;                                // InformationManager
using TaleWorlds.Library;                             // InformationMessage

namespace RealisticEconomy.Models
{
    public class RealisticSettlementEconomyModel : SettlementEconomyModel
    {
        // Instantiate the game's default economy model directly
        private SettlementEconomyModel DefaultModel => new DefaultSettlementEconomyModel();

        // 1) Town gold change
        public override int GetTownGoldChange(Town town)
        {
            int gold = DefaultModel.GetTownGoldChange(town);
            InformationManager.DisplayMessage(
                new InformationMessage($"[RealisticEconomy] {town.Name} daily gold = {gold}")
            );
            return gold;
        }

        // 2) Demand shift per purchase value
        public override float GetDemandChangeFromValue(float purchaseValue)
            => DefaultModel.GetDemandChangeFromValue(purchaseValue);

        // 3) Supply vs demand tuple (supply, demand)
        public override (float, float) GetSupplyDemandForCategory(
            Town town,
            ItemCategory category,
            float dailySupply,
            float dailyDemand,
            float oldSupply,
            float oldDemand)
            => DefaultModel.GetSupplyDemandForCategory(
                   town, category,
                   dailySupply, dailyDemand,
                   oldSupply, oldDemand
               );

        // 4) Estimated demand for one item category
        public override float GetEstimatedDemandForCategory(
            Town town,
            ItemData itemData,
            ItemCategory category)
            => DefaultModel.GetEstimatedDemandForCategory(town, itemData, category);

        // 5) Daily aggregate demand over N days
        public override float GetDailyDemandForCategory(
            Town town,
            ItemCategory category,
            int days)
            => DefaultModel.GetDailyDemandForCategory(town, category, days);
    }
}
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;          // for Settlement, Town
using TaleWorlds.CampaignSystem.ComponentInterfaces; // for IDataStore
using TaleWorlds.CampaignSystem.GameComponents;      // for SettlementFoodStocksComponent
using TaleWorlds.MountAndBlade;                      // for CampaignBehaviorBase, CampaignEvents
using RealisticEconomy.Models;                       // for FileLogger

namespace RealisticEconomy.Behaviors
{
    public class RealisticEconomyBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent
                          .AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.DailyTickSettlementEvent
                          .AddNonSerializedListener(this, OnDailySettlementTick);
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            FileLogger.Clear();
            FileLogger.Log("=== RealisticEconomy Daily Log Started ===");
            starter.AddModel(new RealisticSettlementEconomyModel());
        }

        private void OnDailySettlementTick(Settlement settlement)
        {
            if (settlement.Town == null) return;
            var town = settlement.Town;

            // 1) Gold change
            var econModel = new RealisticSettlementEconomyModel();
            int dailyGoldDelta = econModel.GetTownGoldChange(town);

            // 2) Prosperity
            int prosperity = (int)town.Prosperity;

            // 3) Food stocks (current)
            var foodComp = settlement.GetComponent<SettlementFoodStocksComponent>();
            int currentFood = (int)foodComp.FoodStocks;

            // 4) Food change
            var foodModel = Campaign.Current.Models
                               .GetModel<SettlementFoodModel>();
            int dailyFoodDelta = (int)foodModel
                                    .CalculateTownFoodStocksChange(town)
                                    .ResultNumber;

            // 5) Optional: Garrison size
            int garrisonSize = settlement.GarrisonParty?.MemberRoster.TotalManCount ?? 0;

            // Log everything
            FileLogger.Log(
                $"{town.Name}: " +
                $"Δgold={dailyGoldDelta}, " +
                $"Pros={prosperity}, " +
                $"FoodNow={currentFood}, " +
                $"Δfood={dailyFoodDelta}, " +
                $"Garrison={garrisonSize}"
            );
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
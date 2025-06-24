using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;            // Settlement, Town
using TaleWorlds.CampaignSystem.ComponentInterfaces;    // IDataStore
using TaleWorlds.CampaignSystem.GameComponents;         // DefaultSettlementFoodModel
using TaleWorlds.MountAndBlade;                         // CampaignBehaviorBase, CampaignEvents
using RealisticEconomy.Models;                          // FileLogger, RealisticSettlementEconomyModel

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

            // register *here* so it overwrites the vanilla model
            starter.AddModel(new RealisticSettlementEconomyModel());
        }

        private void OnDailySettlementTick(Settlement settlement)
        {
            if (settlement.Town == null) return;
            var town = settlement.Town;

            // 1) Gold delta via your override
            var econModel = Campaign.Current.Models
                 .GetType()                                      // reflect
                 .GetMethod("GetModel") ??                       // 1.2.x
                 Campaign.Current.Models.GetType()
                        .GetMethod("GetGameModel");              // 1.1.x

            var realEcoModel = econModel != null
                ? econModel.Invoke(Campaign.Current.Models,
                                   new object[] { typeof(SettlementEconomyModel) })
                      as RealisticSettlementEconomyModel
                : null;

            var dailyGoldDelta = realEcoModel?.GetTownGoldChange(town) ?? 0;

            // 2) Prosperity
            int prosperity = (int)town.Prosperity;

            // 3+4) Food stocks & daily delta via DefaultSettlementFoodModel
            var foodModel = new DefaultSettlementFoodModel();
            var foodChange = foodModel.CalculateTownFoodStocksChange(town);
            int dailyFoodDelta = (int)foodChange.ResultNumber;

            // To get “current” food stocks, Bannerlord doesn’t expose it directly;
            // you can approximate with BaseNumber or re‐calculate backward:
            int currentFood = (int)foodChange.BaseNumber;

            // 5) Garrison size directly off Town.GarrisonParty
            int garrisonSize = town.GarrisonParty?.MemberRoster.TotalManCount ?? 0;

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
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;                              // CampaignEvents, Campaign.Current
using TaleWorlds.CampaignSystem.Settlements;                  // Town
using TaleWorlds.CampaignSystem.ComponentInterfaces;          // SettlementEconomyModel, SettlementFoodModel
using TaleWorlds.Core;                                        // ExplainedNumber
using TaleWorlds.SaveSystem;                                  // IDataStore
using RealisticEconomy.Models;                                // ResourceLedger
using RealisticEconomy;                                       // FileLogger

namespace RealisticEconomy.Behaviors
{
    public class ResourceBehavior : CampaignBehaviorBase
    {
        private readonly Dictionary<Town, ResourceLedger> _ledgers
            = new Dictionary<Town, ResourceLedger>();

        public override void RegisterEvents()
        {
            // Subscribe to the town-specific daily tick
            CampaignEvents.DailyTickTownEvent
                .AddNonSerializedListener(this, OnDailyTickTown);
        }

        private void OnDailyTickTown(Town town)
        {
            // 1) Net gold: use the instance property SettlementConsumptionModel
            var econModel = Campaign.Current.Models;
            int goldΔ = econModel.SettlementConsumptionModel
                                .GetTownGoldChange(town);

            // 2) Net food: use the instance property SettlementFoodModel
            ExplainedNumber fn = econModel.SettlementFoodModel
                                        .CalculateTownFoodStocksChange(
                                            town,
                                            includeMarketStocks: true,
                                            includeDescriptions: false
                                        );
            int foodΔ = (int)fn.ResultNumber;

            // 3) Record into our ledger
            _ledgers[town] = new ResourceLedger
            {
                GoldChange = goldΔ,
                FoodChange = foodΔ
            };

            // 4) Debug log
            if (goldΔ != 0 || foodΔ != 0)
                FileLogger.Log($"[Resource] {town.Name}: Δgold={goldΔ}, Δfood={foodΔ}");
        }

        /// <summary>
        /// Retrieve the last recorded deltas for a given town.
        /// </summary>
        public ResourceLedger GetLedger(Town town)
            => _ledgers.TryGetValue(town, out var ledger) ? ledger : null;

        public override void SyncData(IDataStore dataStore) { }
    }
}

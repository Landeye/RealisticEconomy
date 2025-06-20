using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using RealisticEconomy.Models;  // for FileLogger, RealisticSettlementEconomyModel

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
            // Register your model so the game uses it for its own economy ops
            starter.AddModel(new RealisticSettlementEconomyModel());
        }

        private void OnDailySettlementTick(Settlement settlement)
        {
            if (settlement.Town == null) return;
            var town = settlement.Town;

            // Instantiate *your* model and call its override directly:
            var model = new RealisticSettlementEconomyModel();
            int dailyChange = model.GetTownGoldChange(town);

            FileLogger.Log($"{town.Name}: Δ gold = {dailyChange}");
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces; // for starter.AddModel(...)
using TaleWorlds.SaveSystem;                        // for IDataStore
using RealisticEconomy.Models;                      // for RealisticSettlementEconomyModel

namespace RealisticEconomy.Behaviors
{
    public class RealisticEconomyBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
            => CampaignEvents.OnSessionLaunchedEvent
                             .AddNonSerializedListener(this, OnSessionLaunched);

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            // Register your custom economy model so it overrides the built-in one
            // starter.AddModel(new RealisticSettlementEconomyModel());
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;  // for CampaignBehaviorBase
using TaleWorlds.MountAndBlade;                       // for CampaignEvents, CampaignGameStarter
using TaleWorlds.SaveSystem;                          // for IDataStore
using RealisticEconomy.Models;                        // for RealisticSettlementEconomyModel

namespace RealisticEconomy.Behaviors
{
    public class RealisticEconomyBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent
                          .AddNonSerializedListener(this, OnSessionLaunched);
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            // Register your custom economy model
            starter.AddModel(new RealisticSettlementEconomyModel());
        }

        // Mandatory override (no-op)
        public override void SyncData(IDataStore dataStore) { }
    }
}
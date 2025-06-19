using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;  // for SettlementEconomyModel
using TaleWorlds.MountAndBlade;                       // for CampaignBehaviorBase, CampaignEvents, CampaignGameStarter
using TaleWorlds.SaveSystem;                         // for IDataStore
using RealisticEconomy.Models;                       // for RealisticSettlementEconomyModel

namespace RealisticEconomy.Behaviors
{
    public class RealisticEconomyBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            // Fires once the campaign has fully launched
            CampaignEvents.OnSessionLaunchedEvent
                          .AddNonSerializedListener(this, OnSessionLaunched);
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            // **** UNCOMMENTED: inject your custom model ****
            starter.AddModel(new RealisticSettlementEconomyModel());
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
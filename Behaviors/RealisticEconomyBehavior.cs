using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using RealisticEconomy.Models;

namespace RealisticEconomy.Behaviors
{
    public class RealisticEconomyBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            // DEBUG: confirm RegisterEvents is called
            InformationManager.DisplayMessage(
                new InformationMessage("[RealEco] RegisterEvents called")
            );
            CampaignEvents.OnSessionLaunchedEvent
                .AddNonSerializedListener(this, OnSessionLaunched);
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            // DEBUG: confirm OnSessionLaunched fired
            InformationManager.DisplayMessage(
                new InformationMessage("[RealEco] OnSessionLaunched fired")
            );

            // 1) Clear previous logs
            FileLogger.Clear();

            // 2) Inject our custom economy model override
            starter.AddModel(new RealisticSettlementEconomyModel());

            // DEBUG: confirm model added
            InformationManager.DisplayMessage(
                new InformationMessage("[RealEco] Custom model registered")
            );
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
using TaleWorlds.CampaignSystem;                        // for Campaign.Current
using TaleWorlds.CampaignSystem.Settlements;            // for Settlement.Find, Town
using TaleWorlds.CampaignSystem.ComponentInterfaces;    // for SettlementEconomyModel
using TaleWorlds.CampaignSystem.GameComponents;         // for DefaultSettlementEconomyModel
using TaleWorlds.MountAndBlade;                         // for CampaignBehaviorBase, CampaignEvents, CampaignGameStarter
using TaleWorlds.Core;                                  // for InformationManager
using TaleWorlds.Library;                               // for InformationMessage
using TaleWorlds.SaveSystem;                            // for IDataStore
using RealisticEconomy.Models;                          // for RealisticSettlementEconomyModel

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
            // 1) Confirm the behavior is firing
            InformationManager.DisplayMessage(
                new InformationMessage("[RealEco] Behavior fired")
            );

            // 2) Force-test Sargot gold immediately
            var settlement = Settlement.Find("town_sargot");
            if (settlement != null && settlement.Town != null)
            {
                Town sargotTown = settlement.Town;
                var defaultModel = new DefaultSettlementEconomyModel();
                int gold = defaultModel.GetTownGoldChange(sargotTown);

                InformationManager.DisplayMessage(
                    new InformationMessage($"[RealEco] Sargot gold test = {gold}")
                );
            }

            // 3) Register your custom economy model for the daily tick
            starter.AddModel(new RealisticSettlementEconomyModel());
        }

        // Required by CampaignBehaviorBase
        public override void SyncData(IDataStore dataStore) { }
    }
}
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace RealisticEconomy.Behaviors
{
    public class CaravanBiasBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents() =>
            CampaignEvents.DailyTickSettlementEvent
                          .AddNonSerializedListener(this, OnDailyTickSettlement);

        private void OnDailyTickSettlement(Settlement settlement)
        {
            var kingdom = settlement?.OwnerClan?.Kingdom;
            if (kingdom == null) return;

            var tradeDir = Campaign.Current
                                   .GetCampaignBehavior<TradeDirectiveBehavior>();
            if (tradeDir == null) return;

            // ‼ TWO out-params, not one
            Settlement importSettlement;
            Settlement exportSettlement;

            if (!tradeDir.TryGetOrder(kingdom,
                                      out importSettlement,
                                      out exportSettlement))
                return;                        // no directive this week

            foreach (var caravan in settlement.Parties.Where(p => p.IsCaravan))
            {
                float bias =
                    settlement == importSettlement ? 3f :
                    settlement == exportSettlement ? 0.7f : 1f;

                // TODO: choose an actual AI-tuning call here.
                // Example placeholder: adjust initiative weights
                caravan.Ai.SetInitiative(bias, 1f, 6f);
            }
        }

        public override void SyncData(IDataStore _) { }
    }
}


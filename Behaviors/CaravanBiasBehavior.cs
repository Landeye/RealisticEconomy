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
            var tradeDir = Campaign.Current.GetCampaignBehavior<TradeDirectiveBehavior>();
            if (tradeDir == null) return;

            // Do we have an export partner for THIS settlement?
            if (!tradeDir.TryGetOrder(settlement, out Settlement exportSettlement))
                return;

            foreach (var caravan in settlement.Parties.Where(p => p.IsCaravan))
            {
                // Higher bias when parked in exporter, lower in importer
                float bias = settlement == exportSettlement ? 3f : 0.7f;
                caravan.Ai.SetInitiative(bias, 1f, 6f);
            }
        }

        public override void SyncData(IDataStore _) { }
    }
}


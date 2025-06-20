using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;  // for Settlement
using RealisticEconomy.Behaviors;             // for ResourceBehavior

namespace RealisticEconomy.Models
{
    public static class EconomicStressModel
    {
        private const float IdealGoldPerDay = 1000f;
        private const float IdealFoodPerDay = 0f;

        public static float CalculateESI(IFaction faction)
        {
            var beh = Campaign.Current.GetCampaignBehavior<ResourceBehavior>();

            // Loop through each town settlement
            var towns = Settlement.All
                        .Where(s => s.MapFaction == faction && s.IsTown);

            float stress = 0f;
            foreach (var settlement in towns)
            {
                var ledger = beh.GetLedger(settlement);
                if (ledger == null) continue;

                // Gold stress
                if (ledger.GoldChange < IdealGoldPerDay)
                    stress += (IdealGoldPerDay - ledger.GoldChange) / IdealGoldPerDay;

                // Food stress
                if (ledger.FoodChange < IdealFoodPerDay)
                    stress += (IdealFoodPerDay - ledger.FoodChange) / 10f;
            }

            return stress;
        }
    }
}
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;  // for Town
using RealisticEconomy.Behaviors;             // for ResourceBehavior

namespace RealisticEconomy.Models
{
    /// <summary>
    /// Sums normalized daily shortages across all towns of a faction.
    /// </summary>
    public static class EconomicStressModel
    {
        private const float IdealGoldPerDay = 1000f;
        private const float IdealFoodPerDay = 0f;

        public static float CalculateESI(IFaction faction)
        {
            var behavior = Campaign.Current
                                   .GetCampaignBehavior<ResourceBehavior>();

            // 1) Get all TOWNS owned by this faction
            var myTowns = Settlement.All
                          .OfType<Town>()
                          .Where(t => t.MapFaction == faction);

            float stress = 0f;

            // 2) For each town, grab its ledger and compute shortages
            foreach (var town in myTowns)
            {
                var ledger = behavior.GetLedger(town);
                if (ledger == null) continue;

                // Normalize deltas against ideals
                float goldShortage = (IdealGoldPerDay - ledger.GoldChange) / IdealGoldPerDay;
                float foodShortage = (IdealFoodPerDay - ledger.FoodChange) / IdealFoodPerDay;
                // (if IdealFoodPerDay == 0, treat any negative or positive accordingly)
                if (IdealFoodPerDay == 0f)
                {
                    // Any negative foodΔ counts as full stress; positive is zero stress
                    foodShortage = ledger.FoodChange < 0f
                                   ? -ledger.FoodChange / 1000f
                                   : 0f;
                }

                stress += goldShortage + foodShortage;
            }

            return stress;
        }
    }
}

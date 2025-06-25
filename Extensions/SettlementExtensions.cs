using TaleWorlds.CampaignSystem.Settlements;

namespace RealisticEconomy.Extensions
{
    public static class SettlementExtensions
    {
        /// <summary>
        /// Returns a simple ESI value:
        ///   • Positive  = food shortage  (needs imports)
        ///   • Negative  = food surplus   (can export)
        ///   • Zero      = balanced
        /// </summary>
        public static int GetEconomicStressIndex(this Settlement s)
        {
            if (s?.Town == null) return 0;

            // Example formula: inverse of FoodStocks divided by 10
            // Feel free to swap in your real model here.
            float food = s.Town.FoodStocks;          // vanilla food stocks
            return (int)(50 - food / 10f);           // equilibrium around 500 food
        }
    }
}

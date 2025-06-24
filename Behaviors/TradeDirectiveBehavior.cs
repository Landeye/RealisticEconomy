using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;          // for IDataStore
using RealisticEconomy.Models;        // FileLogger (comment out if you don’t have it)

namespace RealisticEconomy.Behaviors
{
    /// <summary>
    /// Nightly audit: chooses the hungriest & most-stocked settlements per kingdom
    /// and stores that pair so caravans know where to haul food.
    /// </summary>
    public class TradeDirectiveBehavior : CampaignBehaviorBase
    {
        // Kingdom → (import, export)
        private readonly Dictionary<Kingdom, (Settlement import, Settlement export)> _orders =
            new Dictionary<Kingdom, (Settlement, Settlement)>();

        private int _lastDay;

        public override void RegisterEvents() =>
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);

        private void OnDailyTick()
        {
            int today = CampaignTime.Now.GetDayOfSeason;
            if (today == _lastDay) return;            // run once per calendar day
            _lastDay = today;

            foreach (Kingdom k in Kingdom.All)
            {
                var fiefs = k.Settlements.Where(s => s.Town != null); // towns & castles

                Settlement import = fiefs.OrderBy(s => s.Town.FoodStocks).FirstOrDefault();
                Settlement export = fiefs.Where(s => s != import && s.Town.FoodStocks > 200)
                                         .OrderByDescending(s => s.Town.FoodStocks)
                                         .FirstOrDefault();

                if (import != null && export != null && import.Town.FoodStocks < 0)
                {
                    _orders[k] = (import, export);
                    FileLogger.Log($"[Directive] {k.Name} → feed {import.Name} "
                                 + $"from {export.Name} "
                                 + $"({import.Town.FoodStocks}/{export.Town.FoodStocks:+0;-0})");
                }
                else
                {
                    _orders.Remove(k);
                }
            }
        }

        /// <summary>
        /// Returns true if the kingdom currently has an active import/export directive.
        /// </summary>
        public bool TryGetOrder(Kingdom k, out Settlement import, out Settlement export)
        {
            if (_orders.TryGetValue(k, out var pair))
            {
                import = pair.import;
                export = pair.export;
                return true;
            }

            import = export = null;
            return false;
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}






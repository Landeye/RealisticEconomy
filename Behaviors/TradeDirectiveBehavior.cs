using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;          // IDataStore
using RealisticEconomy.Models;        // SupplySettings + FileLogger
using RealisticEconomy.Extensions;

namespace RealisticEconomy.Behaviors
{
    public class TradeDirectiveBehavior : CampaignBehaviorBase
    {
        // import → export
        private readonly Dictionary<Settlement, Settlement> _orders =
            new Dictionary<Settlement, Settlement>();

        private int _lastDay;                     // ensures we audit once a day

        //──────────────────────────────────────────
        //  CampaignBehaviorBase boilerplate
        //──────────────────────────────────────────
        public override void RegisterEvents() =>
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);

        public override void SyncData(IDataStore _) { /* nothing to save yet */ }

        //──────────────────────────────────────────
        //  Public helpers for other behaviors
        //──────────────────────────────────────────

        /// <summary>Returns the exporter that should supply <paramref name="import"/>.</summary>
        public bool TryGetOrder(Settlement import, out Settlement export) =>
            _orders.TryGetValue(import, out export);

        /// <summary>Enumerable used by SupplyPartyBehavior to iterate active orders.</summary>
        public static IEnumerable<(Settlement import, Settlement export)> ActiveOrders =>
            _instance?._orders.Select(kv => (kv.Key, kv.Value))
                               ?? Enumerable.Empty<(Settlement, Settlement)>();

        private static TradeDirectiveBehavior _instance;
        public TradeDirectiveBehavior() => _instance = this;

        //──────────────────────────────────────────
        //  Main audit loop
        //──────────────────────────────────────────
        private void OnDailyTick()
        {
            int today = CampaignTime.Now.GetDayOfSeason;
            if (today == _lastDay) return;            // already ran today
            _lastDay = today;

            // 1) HUNGRIEST settlement (must meet ESI_IMPORT_MIN)
            Settlement import = Settlement.All
                .Where(s => s.Town != null &&
                            s.GetEconomicStressIndex() >= SupplySettings.ESI_IMPORT_MIN)
                .OrderByDescending(s => s.GetEconomicStressIndex())
                .FirstOrDefault();

            if (import == null) { _orders.Clear(); return; } // no crisis today

            // 2) BEST exporter (lowest ESI, within radius, not at war)
            Settlement export = Settlement.All
                .Where(s => s.Town != null &&
                            s != import &&
                            s.GetEconomicStressIndex() <= SupplySettings.ESI_EXPORT_MAX &&
                            s.Position2D.DistanceSquared(import.Position2D) <=
                                SupplySettings.MAX_ROUTE_KM * SupplySettings.MAX_ROUTE_KM &&
                            !FactionManager.IsAtWarAgainstFaction(
                                s.OwnerClan.Kingdom, import.OwnerClan.Kingdom))
                .OrderBy(s => s.GetEconomicStressIndex())     // most surplus first
                .FirstOrDefault();

            if (export != null)
            {
                _orders[import] = export;

                // Optional debug output
                FileLogger.Log($"[Directive] feed {import.Name} from {export.Name} " +
                               $"(ESI {import.GetEconomicStressIndex():+0;-0}/" +
                               $"{export.GetEconomicStressIndex():+0;-0})");
            }
            else
            {
                // Couldn’t find a valid exporter today
                _orders.Clear();
            }
        }
    }
}
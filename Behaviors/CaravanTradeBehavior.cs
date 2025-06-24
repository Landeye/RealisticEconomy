using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;      // MobileParty
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using HarmonyLib;                           // AccessTools
using RealisticEconomy.Models;              // FileLogger

namespace RealisticEconomy.Behaviors
{
    public class CaravanTradeBehavior : CampaignBehaviorBase
    {
        private readonly Dictionary<MobileParty, CaravanLedger> _ledgers =
            new Dictionary<MobileParty, CaravanLedger>();

        private int _dayCounter;   // 0-6 → reset window on 6

        private static readonly FieldInfo ProfitField =
            AccessTools.Field(typeof(CaravanPartyComponent), "_lastTradingProfit") ??
            AccessTools.Field(typeof(CaravanPartyComponent), "_tradeProfit");

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, OnDailyTickClan);
        }

        // ───────── midnight processing ─────────
        private void OnDailyTick()
        {
            _dayCounter = (_dayCounter + 1) % 7;

            foreach (MobileParty party in MobileParty.All)
            {
                if (party == null || !party.IsActive) continue;

                var cpc = party.PartyComponent as CaravanPartyComponent;
                if (cpc == null) continue;                    // not a caravan

                float profit = ProfitField != null
                             ? (float)ProfitField.GetValue(cpc)
                             : 0f;

                if (profit != 0f)
                    RecordTrade(party, profit);

                // reset field so we don’t double-count tomorrow
                if (ProfitField != null) ProfitField.SetValue(cpc, 0f);
            }

            // nightly debug dump
            foreach (var kv in _ledgers)
                FileLogger.Log($"[Caravan] {kv.Key.Name} Δweek {kv.Value.WeekSum:+0;-0}");
        }

        // ───────── 7-day bankruptcy check ─────────
        private void OnDailyTickClan(Clan clan)
        {
            if (_dayCounter != 6) return;   // run only once per 7-day window

            foreach (var kv in _ledgers)
            {
                MobileParty party = kv.Key;
                CaravanLedger ledg = kv.Value;

                if (!party.IsActive) continue;
                if (party.ActualClan != clan) continue;

                if (ledg.WeekSum <= 0f)
                {
                    FileLogger.Log($"[Caravan] {party.Name} bankrupt → disband");
                    party.RemoveParty();     // public helper
                    ledg.Reset();
                }
            }
        }

        private void RecordTrade(MobileParty party, float profit)
        {
            CaravanLedger ledger;
            if (!_ledgers.TryGetValue(party, out ledger))
            {
                ledger = new CaravanLedger();
                _ledgers[party] = ledger;
            }
            ledger.AddDayProfit(profit);
        }

        public override void SyncData(IDataStore ds) { }
    }
}



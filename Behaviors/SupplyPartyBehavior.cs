using System.Collections.Generic;
using System.Linq;
using System.Reflection;                              // ← reflection for _spawnTrack
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using RealisticEconomy.Helpers;
using RealisticEconomy.Models;

namespace RealisticEconomy.Behaviors
{
    public class SupplyPartyBehavior : CampaignBehaviorBase
    {
        /* exporter-settlement → last spawn hour (game-hours) */
        private readonly Dictionary<Settlement, double> _lastSpawnHour =
            new Dictionary<Settlement, double>();

        // ──────────────────────────────────────────
        //  CampaignBehaviorBase
        // ──────────────────────────────────────────
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent
                         .AddNonSerializedListener(this, HourlyAudit);

            CampaignEvents.HourlyTickPartyEvent
                         .AddNonSerializedListener(this, TickConvoy);
        }

        public override void SyncData(IDataStore _) { }

        // ──────────────────────────────────────────
        //  Hourly audit – decide whether to spawn
        // ──────────────────────────────────────────
        private void HourlyAudit()
        {
            foreach (var order in TradeDirectiveBehavior.ActiveOrders)
            {
                Settlement import = order.import;
                Settlement export = order.export;

                if (import.Town.FoodStocks > 0) continue;          // crisis solved
                if (!CoolDownOk(export)) continue;           // cooldown timer
                if (ActiveConvoys(export.MapFaction)
                    >= SupplySettings.MAX_CONVOYS_PER_FACTION) continue;

                SpawnConvoy(import, export);
            }
        }

        private bool CoolDownOk(Settlement export)
        {
            double last;
            return !_lastSpawnHour.TryGetValue(export, out last) ||
                   CampaignTime.Now.ToHours - last >= SupplySettings.HOURS_COOLDOWN;
        }

        private static int ActiveConvoys(IFaction faction)
        {
            return MobileParty.All.Count(p =>
                p.PartyComponent is SupplyPartyComponent &&
                p.MapFaction == faction);
        }

        // ──────────────────────────────────────────
        //  Per-convoy tick – unload / despawn / rescue
        // ──────────────────────────────────────────
        private static void TickConvoy(MobileParty party)
        {
            var comp = party.PartyComponent as SupplyPartyComponent;
            if (comp == null) return;

            if (party.CurrentSettlement == comp.Import)
            {
                comp.DeliverSupplies(party);                // unload
                party.Ai.SetMoveGoToSettlement(comp.Export);
            }
            else if (party.CurrentSettlement == comp.Export)
            {
                comp.Despawn(party);                        // done
            }
            else if (party.TargetSettlement == null &&
                     party.CurrentSettlement == null)
            {
                party.Ai.SetMoveGoToSettlement(comp.Import); // restore order
            }
        }

        // ──────────────────────────────────────────
        //  Spawn convoy
        // ──────────────────────────────────────────
        private void SpawnConvoy(Settlement import, Settlement export)
        {
            /* captain hero in exporter’s clan → correct banner */
            CharacterObject baseTroop =
                MBObjectManager.Instance.GetObject<CharacterObject>("imperial_recruit");
            Clan ownerClan = export.OwnerClan ?? Clan.PlayerClan;

            Hero captain = HeroCreator.CreateSpecialHero(
                baseTroop, export, ownerClan, null, 20);
            captain.StringId = "re_convoy_captain_" + MBRandom.RandomInt();
            captain.ChangeState(Hero.CharacterStates.NotSpawned);

            Hero sponsor = ownerClan.Leader;
            var comp = new SupplyPartyComponent(import, export, sponsor, captain);

            MobileParty party = MobileParty.CreateParty(
                                    "re_supply_" + MBRandom.RandomInt(),
                                    comp,
                                    null);

            DisableTracks(party);               // ← prevents TrackPool overflow

            party.InitializeMobilePartyAtPosition(
                TroopRoster.CreateDummyTroopRoster(),
                TroopRoster.CreateDummyTroopRoster(),
                export.Position2D);

            FillRoster(party);
            ItemHelper.BuyFood(party, SupplySettings.SUPPLY_AMOUNT);
            FillRoster(party);                  // after cargo added

            party.Ai.SetDoNotMakeNewDecisions(true);
            party.Ai.SetMoveGoToSettlement(import);

            InformationManager.DisplayMessage(
                new InformationMessage(
                    string.Format("Supply convoy: {0} → {1}",
                                  export.Name, import.Name),
                    Colors.Green));

            _lastSpawnHour[export] = CampaignTime.Now.ToHours;
        }

        // ──────────────────────────────────────────
        //  Helpers
        // ──────────────────────────────────────────
        /* turn off dotted-line track writing for this party */
        private static void DisableTracks(MobileParty p)
        {
            var field = typeof(MobileParty)
                        .GetField("_spawnTrack",
                                  BindingFlags.Instance |
                                  BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(p, false);
        }

        /* ensure escorts, mules, riding horse */
        private static void FillRoster(MobileParty party)
        {
            ItemObject mule = MBObjectManager.Instance.GetObject<ItemObject>("sumpter_horse");
            ItemObject saddle = MBObjectManager.Instance.GetObject<ItemObject>("saddle_horse");
            CharacterObject guard =
                MBObjectManager.Instance.GetObject<CharacterObject>("caravan_guard");

            int need = SupplySettings.MIN_ESCORT + 1 - party.MemberRoster.TotalManCount;
            if (guard != null && need > 0)
                party.MemberRoster.AddToCounts(guard, need);

            if (mule != null)
            {
                int have = party.ItemRoster.GetItemNumber(mule);
                if (have < SupplySettings.MIN_SUMPTERS)
                    party.ItemRoster.AddToCounts(mule,
                        SupplySettings.MIN_SUMPTERS - have);
            }

            if (SupplySettings.ADD_ESCORT_MOUNT && saddle != null &&
                party.ItemRoster.GetItemNumber(saddle) < 2)
            {
                party.ItemRoster.AddToCounts(saddle,
                    2 - party.ItemRoster.GetItemNumber(saddle));
            }
        }
    }
}

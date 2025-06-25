using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.CampaignBehaviors;   // for track clean-up
using System.Reflection;
using RealisticEconomy.Models;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;

namespace RealisticEconomy.Behaviors
{
    internal sealed class SupplyPartyComponent : PartyComponent
    {
        private readonly Settlement _import;
        private readonly Settlement _export;
        private readonly Hero _sponsor;
        private readonly Hero _captain;

        internal SupplyPartyComponent(Settlement import,
                                      Settlement export,
                                      Hero sponsor,
                                      Hero captain)
            : base()                         // parameter-less ctor in 1.2.x
        {
            _import = import;
            _export = export;
            _sponsor = sponsor;
            _captain = captain;
        }

        // ------- abstract properties ---------------------------------
        public override TextObject Name => new TextObject("Supply Convoy");
        public override Settlement HomeSettlement => _export;
        public override Hero PartyOwner => _captain;

        // ------- exposed to SupplyPartyBehavior -----------------------
        internal Settlement Import => _import;
        internal Settlement Export => _export;

        internal void DeliverSupplies(MobileParty party)
        {
            int profit = party.PartyTradeGold;
            int toSponsor = (int)(profit * SupplySettings.SPONSOR_SHARE);

            _sponsor.Gold += toSponsor;
            party.PartyTradeGold = profit - toSponsor;      // captain’s cut

            InformationManager.DisplayMessage(
                new InformationMessage(
                    $"{_sponsor.Name} +{toSponsor} denars (convoy profit)",
                    Colors.Green));
        }

        internal void Despawn(MobileParty party)
        {
            ReleaseTracks(party);                          // tidy track pool
            KillCharacterAction.ApplyByRemove(party.LeaderHero);
            party.RemoveParty();
        }

        // ------- optional track clean-up helper -----------------------
        private static void ReleaseTracks(MobileParty party)
        {
            var mt = Campaign.Current
                      .GetCampaignBehavior<MapTracksCampaignBehavior>();
            if (mt == null) return;

            typeof(MapTracksCampaignBehavior)
              .GetMethod("RemoveTracksOfParty",
                         BindingFlags.Instance | BindingFlags.NonPublic)?
              .Invoke(mt, new object[] { party });
        }
    }
}


using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace RealisticEconomy.Helpers
{
    public static class ItemHelper
    {
        /// <summary>Add grain to the party (simulated “buy”).</summary>
        public static void BuyFood(MobileParty party, int amount)
        {
            party.Party.ItemRoster.AddToCounts(DefaultItems.Grain, amount);
        }

        /// <summary>Remove all grain from the party and add it to the town’s market.</summary>
        public static void SellAllFood(MobileParty seller, Settlement town)
        {
            var roster = seller.Party.ItemRoster;
            int grainQty = roster.GetItemNumber(DefaultItems.Grain);
            if (grainQty == 0) return;

            roster.AddToCounts(DefaultItems.Grain, -grainQty);
            town.Party.ItemRoster.AddToCounts(DefaultItems.Grain, grainQty);
        }
    }
}

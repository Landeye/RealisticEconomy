using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace RealisticEconomy.Helpers
{
    public static class SupplyTemplateHelper
    {
        /// <summary>
        /// Adds four sumpter horses (pack animals) to the party.
        /// Works across all Bannerlord 1.2.x versions by
        /// looking the item up via its string ID.
        /// </summary>
        public static void InitBasicMuleParty(MobileParty party)
        {
            // Look up the pack animal by its item ID
            ItemObject sumpter = Game.Current.ObjectManager
                                        .GetObject<ItemObject>("sumpter_horse");

            if (sumpter != null)
            {
                // Add four animals to the party’s item roster
                party.Party.ItemRoster.AddToCounts(sumpter, 4);
            }

            // Force the game to refresh the party’s visual on the map
            party.Party.SetVisualAsDirty();
        }
    }
}

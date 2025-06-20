using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace RealisticEconomy.Behaviors
{
    public static class EconomyManager
    {
        public static readonly Dictionary<IFaction, long> FactionTreasuries =
            new Dictionary<IFaction, long>();

        public static void Initialize()
        {
            FactionTreasuries.Clear();
            foreach (IFaction f in Campaign.Current.Factions)
            {
                long startingGold = 0;
                if (f.Leader != null)
                    startingGold = f.Leader.Gold;
                FactionTreasuries[f] = startingGold;
            }
        }

        public static bool TryTransfer(IFaction from, IFaction to, int amount)
        {
            long balance;
            if (!FactionTreasuries.TryGetValue(from, out balance) || balance < amount)
                return false;

            FactionTreasuries[from] = balance - amount;
            long toBalance;
            FactionTreasuries.TryGetValue(to, out toBalance);
            FactionTreasuries[to] = toBalance + amount;
            return true;
        }
    }
}
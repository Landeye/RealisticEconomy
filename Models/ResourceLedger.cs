using System.Collections.Generic;

namespace RealisticEconomy.Models
{
    /// <summary>Holds one day’s resource deltas for a town.</summary>
    public class ResourceLedger
    {
        /// <summary>Income minus expenses.</summary>
        public int GoldChange;

        /// <summary>Net change in food‐stock.</summary>
        public int FoodChange;
    }
}

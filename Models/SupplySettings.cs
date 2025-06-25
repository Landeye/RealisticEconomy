namespace RealisticEconomy.Models
{
    public static class SupplySettings
    {
        /* existing values … */
        public const int HOURS_COOLDOWN = 24;
        public const int MAX_CONVOYS_PER_FACTION = 4;
        public const float SPONSOR_SHARE = 0.80f;
        public const float CAPTAIN_SHARE = 0.20f;
        public const int MIN_ESCORT = 5;
        public const int MIN_SUMPTERS = 8;
        public const bool ADD_ESCORT_MOUNT = true;
        public const bool SHOW_CONVOYS = true;
        public const int SUPPLY_AMOUNT = 50;

        /* ─── values TradeDirectiveBehavior expects ─── */
        public const float ESI_IMPORT_MIN = 10f;   // starving threshold
        public const float ESI_EXPORT_MAX = -5f;   // surplus threshold
        public const float MAX_ROUTE_KM = 120f;  // max distance exporter→importer
    }
}



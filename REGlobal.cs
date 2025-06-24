namespace RealisticEconomy
{
    public static class REGlobal
    {
        // Multipliers you can tune in XML or code
        public static float TaxIncomeMultiplier = 1.0f;
        public static float WorkshopIncomeMultiplier = 1.0f;
        public static float GarrisonExpenseMultiplier = 1.0f;

        public static void LoadDefaults()
        {
            // TODO: Load these from an XML config if desired
            TaxIncomeMultiplier = 1.0f;
            WorkshopIncomeMultiplier = 1.0f;
            GarrisonExpenseMultiplier = 1.0f;
        }
    }
}

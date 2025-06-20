using System;
using System.IO;
using TaleWorlds.CampaignSystem.Settlements; // for Town

namespace RealisticEconomy.Models
{
    public static class FileLogger
    {
        private static readonly string CsvPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mount and Blade II Bannerlord",
            "EconomyDebug.csv"
        );

        /// <summary>
        /// Wipe out any old CSV and write the minimal header.
        /// Call once at session start via the static ctor of your model.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CsvPath));
                File.WriteAllText(
                    CsvPath,
                    "Time,SettlementId,DeltaGold,Prosperity" + Environment.NewLine
                );
            }
            catch { }
        }

        /// <summary>
        /// Append a line: HH:mm:ss, settlementID, Δgold, prosperity.
        /// </summary>
        public static void LogTick(Town town, int deltaGold)
        {
            try
            {
                string time = DateTime.Now.ToString("HH:mm:ss");
                string line = $"{time},{town.StringId},{deltaGold},{(int)town.Prosperity}";
                File.AppendAllText(CsvPath, line + Environment.NewLine);
            }
            catch { }
        }
    }
}
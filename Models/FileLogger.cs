using System;
using System.IO;

namespace RealisticEconomy.Models
{
    public static class FileLogger
    {
        private static readonly string _path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mount and Blade II Bannerlord",
            "EconomyDebug.txt"
        );

        public static void Log(string line)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path));
                File.AppendAllText(
                    _path,
                    $"[{DateTime.Now:HH:mm:ss}] {line}{Environment.NewLine}"
                );
            }
            catch { /* swallow errors */ }
        }

        public static void Clear()
        {
            try
            {
                File.WriteAllText(_path, string.Empty);
            }
            catch { }
        }
    }
}
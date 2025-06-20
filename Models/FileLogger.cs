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

        public static void Clear()
        {
            try
            {
                var dir = Path.GetDirectoryName(_path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(_path, string.Empty);
            }
            catch { }
        }

        public static void Log(string line)
        {
            try
            {
                var dir = Path.GetDirectoryName(_path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.AppendAllText(
                    _path,
                    "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] "
                    + line
                    + Environment.NewLine
                );
            }
            catch { }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.SQLite;

namespace LongJohnSilver.Statics
{
    public static class BotSetupHandler
    {
        public static string BotToken { get; private set; }
        public static string WeatherKey { get; private set; }
        public static string GeoKey { get; private set; }

        public static void LoadKeys()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var currentConfig = $@"{currentDirectory}{Path.DirectorySeparatorChar}config";

            if (!File.Exists(currentConfig))
            {
                throw new Exception($"Config File is Missing: {currentConfig}");
            }

            var lines = System.IO.File.ReadAllLines(currentConfig);

            if (lines.Length != 3)
            {
                throw new Exception($"Config File is Corrupt");
            }

            BotToken = lines[0];
            WeatherKey = lines[1];
            GeoKey = lines[2];            
        }
    }
}

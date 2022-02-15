using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace IDEG_DiaTrainer.Config
{
    public class ConfigMgr
    {
        public struct ConfigDesc
        {
            public string ConfigName { get; set; }
            public string ConfigDescription { get; set; }
        }

        public static string ReadConfig(string plainName)
        {
            var assembly = typeof(ConfigMgr).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IDEG_DiaTrainer.Config." + plainName + ".ini");

            if (stream == null)
                return null;

            string text = "";
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            text = text.Replace("{{DataDir}}", Microsoft.Maui.Essentials.FileSystem.AppDataDirectory);

            return text;
        }

        private static string ReadResourceFile(string name)
        {
            string text = "";

            try
            {
                var baseDir = Microsoft.Maui.Essentials.FileSystem.AppDataDirectory;

                using (var reader = new StreamReader(baseDir + "/" + name))
                {
                    text = reader.ReadToEnd();
                }
            }
            catch
            {
            }

            return text;
        }

        private static List<string> PatternParams = new List<string>() {
            "params_5.ini", "params_10.ini", "params_15.ini", "params_20.ini", "params_25.ini", "params_30.ini"
        };

        public static bool ResourcesNeedsUpdate()
        {
            foreach (var parfile in PatternParams)
            {
                string tmp = ReadResourceFile(parfile);

                if (tmp.Length == 0 || tmp != Helpers.ResourceHelper.ReadNamedResource(parfile))
                    return true;
            }

            return false;
        }

        public static void UpdateResources()
        {
            foreach (var parfile in PatternParams)
            {
                string str = Helpers.ResourceHelper.ReadNamedResource(parfile);

                var baseDir = Microsoft.Maui.Essentials.FileSystem.AppDataDirectory;

                using (StreamWriter file = new StreamWriter(baseDir + "/" + parfile, false))
                {
                    file.Write(str);
                }
            }
        }

        public static List<ConfigDesc> GetAvailableTrainingConfigurations()
        {
            List<ConfigDesc> res = new List<ConfigDesc>();

            res.Add(new ConfigDesc { ConfigName = "config-s2013", ConfigDescription = "S2013-based training" });
            res.Add(new ConfigDesc { ConfigName = "config-s2013-lgs", ConfigDescription = "S2013-based training with LGS" });
            res.Add(new ConfigDesc { ConfigName = "config-s2013-betapid2", ConfigDescription = "S2013-based training with BetaPID2" });
            res.Add(new ConfigDesc { ConfigName = "config-s2013-showroom1", ConfigDescription = "S2013 Showroom 1" });

            return res;
        }

        public static List<ConfigDesc> GetAvailableLiveConfigurations()
        {
            List<ConfigDesc> res = new List<ConfigDesc>();

            res.Add(new ConfigDesc { ConfigName = "config-live", ConfigDescription = "Sensor-based" });

            return res;
        }
    }
}

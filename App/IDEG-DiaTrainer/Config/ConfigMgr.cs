using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace IDEG_DiaTrainer.Config
{
    /// <summary>
    /// Configuration manager - manages SmartCGMS configs
    /// </summary>
    public class ConfigMgr
    {
        /// <summary>
        /// Reads given config
        /// The config needs to be bundled into solution and markes as an "EmbeddedResource"
        /// </summary>
        /// <param name="plainName">filename of config (without path and extension)</param>
        /// <returns></returns>
        public static string ReadConfig(string plainName)
        {
            // retrieve config resource stream
            var assembly = typeof(ConfigMgr).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IDEG_DiaTrainer.Config." + plainName + ".ini");

            if (stream == null)
                return null;

            // read everything into a string
            string text = "";
            using (var reader = new StreamReader(stream))
                text = reader.ReadToEnd();

            // substitute DataDir with actual application directory
            // this is there to ensure the paths are safe for every environment - .NET is able to offer a safe directory
            // but we cannot request it directly from C++ code
            text = text.Replace("{{DataDir}}", Microsoft.Maui.Essentials.FileSystem.AppDataDirectory);

            return text;
        }
    }
}

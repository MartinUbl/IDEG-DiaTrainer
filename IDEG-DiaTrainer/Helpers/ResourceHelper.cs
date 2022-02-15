using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    public class ResourceHelper
    {
        public static Stream GetNamedStream(string name)
        {
            try
            {
                var assembly = typeof(ResourceHelper).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(name);

                return stream;
            }
            catch
            {
                //
            }

            return null;
        }

        public static StreamReader GetNamedReader(string name)
        {
            try
            {
                var assembly = typeof(ResourceHelper).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(name);

                if (stream == null)
                    return null;

                return new StreamReader(stream);
            }
            catch
            {
                //
            }

            return null;
        }

        public static string ReadNamedResource(string name)
        {
            string text = "";

            try
            {
                using (var reader = GetNamedReader(name))
                {
                    text = reader.ReadToEnd();
                }
            }
            catch
            {
                //
            }

            return text;
        }
    }
}

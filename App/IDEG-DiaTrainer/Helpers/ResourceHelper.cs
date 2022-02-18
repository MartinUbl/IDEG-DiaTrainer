using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    /// <summary>
    /// Utility class for resource management
    /// </summary>
    public class ResourceHelper
    {
        /// <summary>
        /// Retrieves a stream for a given resource
        /// </summary>
        /// <param name="name">full path to resource</param>
        /// <returns>stream of given resource</returns>
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

        /// <summary>
        /// Retrieves a streamreader for a given resource
        /// </summary>
        /// <param name="name">full path to resource</param>
        /// <returns>streamreader of a given resource</returns>
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

        /// <summary>
        /// Reads all contents of a given resource into a string
        /// </summary>
        /// <param name="name">full path to resource</param>
        /// <returns>resource contents in a string</returns>
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

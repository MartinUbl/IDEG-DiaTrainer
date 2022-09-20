using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    public class TutorialRecord
    {
        public string Identifier { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }

    /// <summary>
    /// Tutorial manager class - loads available tutorials from resources
    /// </summary>
    public class TutorialManager
    {
        private static TutorialManager _Instance = null;

        public static TutorialManager Current { get {
                if (_Instance == null)
                    _Instance = new TutorialManager();
                return _Instance;
        } }

        // list of all loaded records
        private Dictionary<string, List<TutorialRecord>> Records = new Dictionary<string, List<TutorialRecord>>();

        private TutorialManager()
        {
            //
        }

        /// <summary>
        /// Retrieves all loaded tutorials for given section
        /// </summary>
        /// <returns>loaded tutorial records</returns>
        public List<TutorialRecord> GetTutorials(string section)
        {
            if (Records.ContainsKey(section))
                return Records[section];

            if (!Load(section))
                return null;

            return Records[section];
        }

        /// <summary>
        /// Loads all available tutorials of a section from internal resource stream
        /// </summary>
        /// <returns></returns>
        public bool Load(string section)
        {
            try
            {
                // get resource stream
                using (var reader = ResourceHelper.GetNamedReader("IDEG_DiaTrainer.Resources.Content.Tutorial.tutorial-"+section+".csv"))
                {
                    // we use semicolon separator and UTF-8 encoding for the csv file
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };

                    // read all contents
                    using (var csv = new CsvReader(reader, config))
                    {
                        Records[section] = new List<TutorialRecord>();

                        // read header - Read + ReadHeader is used as instructed in CsvReader manual
                        csv.Read();
                        csv.ReadHeader();

                        //identifier;title;text

                        // read all records
                        while (csv.Read())
                        {
                            // try to create TutorialRecord from all lines
                            var record = new TutorialRecord
                            {
                                Identifier = csv.GetField("identifier"),
                                Title = csv.GetField("title"),
                                Text = csv.GetField("text")
                            };

                            Records[section].Add(record);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

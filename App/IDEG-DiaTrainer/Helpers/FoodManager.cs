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
    /// <summary>
    /// Food manager class - loads available food from resources
    /// </summary>
    public class FoodManager
    {
        private static FoodManager _Instance = null;

        public static FoodManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new FoodManager();
                return _Instance;
            }
        }

        // list of all loaded records
        private List<FoodRecord> Records;

        private FoodManager()
        {
            //
        }

        /// <summary>
        /// guarded read from CSV file
        /// </summary>
        /// <param name="csv">given reader</param>
        /// <param name="name">field name</param>
        /// <returns>content of the field</returns>
        /// <returns>null if the field does not contain double</returns>
        private double? ReadNull(CsvReader csv, string name)
        {
            try
            {
                return csv.GetField<double>(name);
            }
            catch
            {
                //
            }

            return null;
        }

        /// <summary>
        /// Retrieves all loaded food
        /// </summary>
        /// <returns>loaded food records</returns>
        public List<FoodRecord> GetFood()
        {
            return Records;
        }

        /// <summary>
        /// Loads all available food from internal resource stream
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            try
            {
                // get resource stream
                using (var reader = ResourceHelper.GetNamedReader("IDEG_DiaTrainer.Resources.Content.Food.food.csv"))
                {
                    // we use semicolon separator and UTF-8 encoding for the csv file
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };

                    // read all contents
                    using (var csv = new CsvReader(reader, config))
                    {
                        Records = new List<FoodRecord>();

                        // read header - Read + ReadHeader is used as instructed in CsvReader manual
                        csv.Read();
                        csv.ReadHeader();

                        //id;name;img;baseamount;portionamount;unit;calories;carbohydrates;sugar;fat;proteins;fibre

                        // read all records
                        while (csv.Read())
                        {
                            // try to create FoodRecords from all lines
                            var record = new FoodRecord
                            {
                                Identifier = csv.GetField("id"),
                                Name = csv.GetField("name"),
                                ImageFilename = csv.GetField("img"),
                                BaseAmount = csv.GetField<double>("baseamount"),
                                PortionAmount = csv.GetField<double>("portionamount"),
                                Units = csv.GetField("unit"),

                                Calories = ReadNull(csv, "calories"),
                                Carbohydrates = ReadNull(csv, "carbohydrates"),
                                Sugar = ReadNull(csv, "sugar"),
                                Fat = ReadNull(csv, "fat"),
                                Proteins = ReadNull(csv, "proteins"),
                                Fibre = ReadNull(csv, "fibre"),
                            };

                            Records.Add(record);
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

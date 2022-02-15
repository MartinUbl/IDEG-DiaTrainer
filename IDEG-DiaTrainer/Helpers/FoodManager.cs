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
    public class FoodManager
    {
        public class FoodRecord
        {
            public string Identifier { get; set; }
            
            public string Name { get; set; }

            public string ImageFilename { get; set; }

            public string Units { get; set; }

            public double? Calories { get; set; }

            public double? Carbohydrates { get; set; }

            public double? Sugar { get; set; }

            public double? Fat { get; set; }

            public double? Proteins { get; set; }

            public double? Fibre { get; set; }
        }

        private List<FoodRecord> Records;

        public FoodManager()
        {
            //
        }

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

        public List<FoodRecord> GetFood()
        {
            return Records;
        }

        public bool Load()
        {
            try
            {
                using (var reader = ResourceHelper.GetNamedReader("IDEG_DiaTrainer.Resources.Content.Food.food.csv"))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };

                    using (var csv = new CsvReader(reader, config))
                    {
                        Records = new List<FoodRecord>();

                        csv.Read();
                        csv.ReadHeader();

                        //id;name;img;unit;calories;carbohydrates;sugar;fat;proteins;fibre

                        while (csv.Read())
                        {
                            var record = new FoodRecord
                            {
                                Identifier = csv.GetField("id"),
                                Name = csv.GetField("name"),
                                ImageFilename = csv.GetField("img"),
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

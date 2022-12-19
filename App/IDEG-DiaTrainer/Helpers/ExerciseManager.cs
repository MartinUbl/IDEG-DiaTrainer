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
    /// Exercise manager class - loads available exercises from resources
    /// </summary>
    public class ExerciseManager
    {
        private static ExerciseManager _Instance = null;

        public static ExerciseManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ExerciseManager();
                return _Instance;
            }
        }

        // list of all loaded records
        private List<ExerciseRecord> Records;

        public ExerciseManager()
        {
            //
        }

        /// <summary>
        /// Retrieves all loaded exercise
        /// </summary>
        /// <returns>loaded exercise records</returns>
        public List<ExerciseRecord> GetExercise()
        {
            return Records;
        }

        /// <summary>
        /// Loads all available exercises from internal resource stream
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            try
            {
                // get resource stream
                using (var reader = ResourceHelper.GetNamedReader("IDEG_DiaTrainer.Resources.Content.Exercise.exercise.csv"))
                {
                    // we use semicolon separator and UTF-8 encoding for the csv file
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };

                    // read all contents
                    using (var csv = new CsvReader(reader, config))
                    {
                        Records = new List<ExerciseRecord>();

                        // read header - Read + ReadHeader is used as instructed in CsvReader manual
                        csv.Read();
                        csv.ReadHeader();

                        //id;name;img;intensity;recommended-duration-minutes

                        // read all records
                        while (csv.Read())
                        {
                            // try to create ExerciseRecord from all lines
                            var record = new ExerciseRecord
                            {
                                Identifier = csv.GetField("id"),
                                Name = csv.GetField("name"),
                                ImageFilename = csv.GetField("img"),
                                Intensity = csv.GetField<double>("intensity"),
                                RecommendedDuration = csv.GetField<int>("recommended-duration-minutes")
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

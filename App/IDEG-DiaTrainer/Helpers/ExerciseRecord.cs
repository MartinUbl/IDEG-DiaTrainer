using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    /// <summary>
    /// ExerciseRecord class - stores information about exercise
    /// </summary>
    public class ExerciseRecord
    {
        // unique identifier - not displayed in any outputs
        public string Identifier { get; set; }

        // exercise name - displayed as a title
        public string Name { get; set; }

        // filename of image, may be empty
        public string ImageFilename { get; set; }

        // usual exercise intensity
        public double? Intensity { get; set; }

        // recommended exercise duration in minutes
        public int? RecommendedDuration { get; set; }
    }
}

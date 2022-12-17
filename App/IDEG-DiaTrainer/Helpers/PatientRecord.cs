using IDEG_DiaTrainer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    /// <summary>
    /// Patient record container
    /// </summary>
    public class PatientRecord
    {
        // unique patient identifier
        public int Id { get; set; }

        // name of the patient
        public String Name { get; set; }

        // age of the patient
        public int Age { get; set; }

        // diabetes type
        public DiabetesType Diabetes { get; set; }

        // stored params string relevant for given model
        public string ParameterString { get; set; }
    }
}

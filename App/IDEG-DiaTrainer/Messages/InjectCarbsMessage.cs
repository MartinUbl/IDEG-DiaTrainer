using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Messages
{
    /// <summary>
    /// Signal message for injecting carbohydrates into simulation
    /// </summary>
    public class InjectCarbsMessage
    {
        public static readonly string Name = "InjectCarbsMessage";

        // amount of carbohydrates
        public double CarbAmount { get; set; } = 0;

        // when to inject - if null, current simulation time is used
        public DateTime? When { get; set; } = null;

        // is this a rescue carbohydrate dosage?
        public Boolean IsRescue { get; set; } = false;
    }
}

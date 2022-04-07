using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Messages
{
    public class InjectExerciseMessage
    {
        public static readonly string Name = "InjectExerciseMessage";

        // exercise intensity
        public double Intensity { get; set; } = 0;

        // when to inject - if null, current simulation time is used
        public DateTime? When { get; set; } = null;

        public int CancelAfterSeconds { get; set; } = 300;
    }
}

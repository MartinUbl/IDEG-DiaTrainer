using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Messages
{
    public class InjectBolusMessage
    {
        public static readonly string Name = "InjectBolusMessage";

        // amount of bolus insulin
        public double BolusAmount { get; set; } = 0;

        // when to inject - if null, current simulation time is used
        public DateTime? When { get; set; } = null;
    }
}

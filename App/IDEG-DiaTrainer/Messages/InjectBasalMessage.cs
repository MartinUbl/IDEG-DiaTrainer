using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Messages
{
    public class InjectBasalMessage
    {
        public static readonly string Name = "InjectBasalMessage";

        // basal insulin rate
        public double BasalRate { get; set; } = 0;

        // when to set - if null, current simulation time is used
        public DateTime? When { get; set; } = null;
    }
}

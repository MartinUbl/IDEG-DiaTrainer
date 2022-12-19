using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Messages
{
    /// <summary>
    /// Message signaling simulation shutdown
    /// </summary>
    public class SimulationSpeedChangeMessage
    {
        public static readonly string Name = "SimulationSpeedChangeMessage";

        public bool Faster { get; set; } = false;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Messages
{
    /// <summary>
    /// Message signaling a new value; this is sent by a simulation controller
    /// </summary>
    public class ValueAvailableMessage
    {
        public static readonly string Name = "ValueAvailableMessage";

        // signal id of new value
        public Guid SignalId { get; set; }

        // date and time of value arrival
        public DateTime DeviceTime { get; set; }

        // the value itself
        public double Value { get; set; }
    }
}

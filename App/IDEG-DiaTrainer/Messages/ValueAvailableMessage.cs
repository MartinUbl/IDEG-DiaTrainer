using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Messages
{
    public class ValueAvailableMessage
    {
        public static readonly string Name = "ValueAvailableMessage";

        public Guid SignalId { get; set; }

        public DateTime DeviceTime { get; set; }

        public double Value { get; set; }
    }
}

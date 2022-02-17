using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Messages
{
    public class InjectCarbsMessage
    {
        public static readonly string Name = "InjectCarbsMessage";

        public double CarbAmount { get; set; } = 0;

        public DateTime? When { get; set; } = null;

        public Boolean IsRescue { get; set; } = false;
    }
}

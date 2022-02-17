using System;
using System.Collections.Generic;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public enum EventCode : byte
    {
        Nothing = 0,
        Shut_Down,
        Level,
        Masked_Level,
        Parameters,
        Parameters_Hint,

        Suspend_Parameter_Solving,
        Resume_Parameter_Solving,
        Solve_Parameters,
        Time_Segment_Start,
        Time_Segment_Stop,
        Warm_Reset,

        Information,
        Warning,
        Error
    };
}

using System;
using System.Collections.Generic;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public class HResult
    {
        public static readonly int S_OK = 0;
        public static readonly int S_FALSE = 1;

        public static bool Succeeded(int res)
        {
            return res == S_OK || res == S_FALSE;
        }
    }
}

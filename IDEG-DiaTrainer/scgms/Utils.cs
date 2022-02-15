using System;
using System.Collections.Generic;
using System.Text;

namespace IDEG_DiaTrainer.scgms
{
    public class Utils
    {
        private static readonly long diffFrom1970To1900 = 2209161600000;
        private static readonly double MSecsPerDay = 24.0 * 60.0 * 60.0 * 1000.0;
        private static readonly double InvMSecsPerDay = 1.0 / MSecsPerDay;

        public static double UnixTimeToRatTime(long unixTime)
        {
            long diff = unixTime * 1000 + diffFrom1970To1900;
            return diff * InvMSecsPerDay;
        }

        public static long RatTimeToUnixTime(double rt)
        {
            double diff = rt * MSecsPerDay;
            long msecs = ((long)diff) - diffFrom1970To1900;

            return msecs / 1000;
        }

        public static double DateTimeToRatTime(DateTime dt)
        {
            return UnixTimeToRatTime((long)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        public static DateTime RatTimeToDateTime(double ratTime)
        {
            DateTime b = new DateTime(1970, 1, 1);
            b.AddSeconds(RatTimeToUnixTime(ratTime));

            return b;
        }
    }
}

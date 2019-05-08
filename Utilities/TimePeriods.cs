using System;
using System.Collections.Generic;

namespace ReactRedux.Utilities {
    internal static class TimePeriods {
        public static IList<string> AllTimePeriods => new List<string>(new [] { "24h", "48h", "7d", "14d" });

        public static bool IsDateWithinBoundry(DateTime dt, DateTime first, string timeSpan) {
            TimeSpan ts = first.Subtract(dt);
            double secsDay = 24 * 60 * 60;
            if (timeSpan == "24h") {
                return ts.TotalSeconds < secsDay;
            } else if (timeSpan == "48h") {
                return ts.TotalSeconds < 2 * secsDay;
            } else if (timeSpan == "7d") {
                return ts.TotalSeconds < 7 * secsDay;
            }
            return ts.TotalSeconds < 14 * secsDay;
        }
    }
}
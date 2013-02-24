using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Common;

namespace System
{
    public static class DateTimeExtensions
    {
        public static bool IsInBetweenInclusive(this DateTime dt, DateTime start, DateTime end)
        {
            return (start <= dt && dt <= end);
        }

        public static string ToShortDateStringOrToday(this DateTime? dt)
        {
            if (!dt.HasValue) return string.Empty;

            return !dt.Value.IsToday() ? dt.Value.ToShortDateString() : Strings.Date_Today;
        }

        public static string ToShortDateTimeStringOrToday(this DateTime? dt)
        {
            if (!dt.HasValue) return string.Empty;

            var datePart = !dt.Value.IsToday() ?
                dt.Value.ToShortDateString() : Strings.Date_Today;

            return datePart + ", " + dt.Value.ToShortTimeString();
        }

        public static string ToLongDateTimeStringOrToday(this DateTime? dt)
        {
            if (!dt.HasValue) return string.Empty;

            var datePart = !dt.Value.IsToday() ?
                dt.Value.ToShortDateString() : Strings.Date_Today;

            return datePart + ", " + dt.Value.ToString("HH:mm:ss");
        }

        public static DateTime AddWorkingDays(this DateTime dt, int workingDays)
        {
            var direction = workingDays < 0 ? -1 : 1;
            for (int i = 0; i < Math.Abs(workingDays); i++)
            {
                dt = dt.AddDays(direction);
                while (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    dt = dt.AddDays(direction);
                }
            }

            return dt;
        }
    }
}

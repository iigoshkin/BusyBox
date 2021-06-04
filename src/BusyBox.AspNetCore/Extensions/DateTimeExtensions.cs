using System;

namespace BusyBox.AspNetCore.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Return the number of seconds that have elapsed since 1970-01-01T00:00:00Z
        /// </summary>
        /// <returns>Return the number of seconds that have elapsed since 1970-01-01T00:00:00Z</returns>
        public static long ToUnixTotalSeconds(this DateTime time)
        {
            DateTime epochUnix = new (1970, 1, 1);
            TimeSpan span = time.Subtract(epochUnix);
            return Convert.ToInt64(span.TotalSeconds);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YouConf.Extensions
{
    /// <summary>
    /// http://stackoverflow.com/questions/7577389/how-to-elegantly-deal-with-timezones
    /// </summary>
    public static class DateTimeExtensions
    {
        public static DateTime UtcToLocal(this DateTime source,
            TimeZoneInfo localTimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(source, localTimeZone);
        }

        public static DateTime LocalToUtc(this DateTime source,
            TimeZoneInfo localTimeZone)
        {
            source = DateTime.SpecifyKind(source, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(source, localTimeZone);
        }
    }
}
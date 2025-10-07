using System;
using System.Globalization;

namespace GameServer.Utils
{
    public class TimeUtils
    {
        public static DateTime Now => DateTime.Now;

        public static DateTime DayStart => Now.Date;
        public static DateTime YesterdayStart => Now.Date.AddDays(-1);
        public static DateTime ThisWeekStart => GetWeekStart(Now);
        public static DateTime LastWeekStart => GetWeekStart(Now.AddDays(-7));
        public static DateTime ThisMonthStart => new DateTime(Now.Year, Now.Month, 1);
        public static int SecondsAgo(DateTime date) => (int)(Now - date).TotalSeconds;

        private static DateTime GetWeekStart(DateTime date) => date.AddDays(
            ((date.DayOfWeek - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek + 7) % 7) * -1
            ).Date;
    }
}

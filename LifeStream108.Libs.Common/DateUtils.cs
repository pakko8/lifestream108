using System;

namespace LifeStream108.Libs.Common
{
    public static class DateUtils
    {
        public static DateTime GetBeginOfDay(DateTime dt)
        {
            return dt.Date;
        }

        public static DateTime GetBeginOfNextDay(DateTime dt)
        {
            return dt.Date.AddDays(1);
        }
    }
}

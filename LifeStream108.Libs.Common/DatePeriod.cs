using System;

namespace LifeStream108.Libs.Common
{
    public class DatePeriod
    {
        public DatePeriod(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        public DateTime From { get; private set; }

        public DateTime To { get; private set; }
    }
}

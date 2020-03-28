using LifeStream108.Libs.Common.Grammar;
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

        public override string ToString()
        {
            int countDays = (int)Math.Ceiling((To - From).TotalDays);
            if (To.Hour == 0 && To.Minute == 0 && To.Second == 0) countDays += 1;
            if (countDays == 0) countDays = 1;

            return $"{From.ToString("dd.MM.yyyy")} {(To != From ? " - " + To.ToString("dd.MM.yyyy") : "")} " +
                $"({countDays} {Declanations.DeclineByNumeral(countDays, "день", "дня", "дней")})";
        }
    }
}

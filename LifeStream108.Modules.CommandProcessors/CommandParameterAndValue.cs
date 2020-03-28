using LifeStream108.Libs.Common;
using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Entities.CommandEntities;
using System;
using System.Collections.Generic;

namespace LifeStream108.Modules.CommandProcessors
{
    public class CommandParameterAndValue
    {
        private readonly static Dictionary<string, int> _dicMonthNames = new Dictionary<string, int>()
        {
            { "JANUARY", 1 },
            { "JAN", 1 },
            { "FEBRUARY", 2 },
            { "FEB", 2 },
            { "MARCH", 3 },
            { "MAR", 3 },
            { "APRIL", 4 },
            { "APR", 4 },
            { "MAY", 5 },
            { "JUNE", 6 },
            { "JUN", 6 },
            { "JULY", 7 },
            { "JUL", 7 },
            { "AUGUST", 8 },
            { "AUG", 8 },
            { "SEPTEMBER", 9 },
            { "SEP", 9 },
            { "OCTOBER", 10 },
            { "OCT", 10 },
            { "NOVEMBER", 11 },
            { "NOV", 11 },
            { "DECEMBER", 12 },
            { "DEC", 12 }
        };

        public CommandParameter Parameter { get; set; }

        public string Value { get; set; }

        public int IntValue => int.Parse(Value);

        public DateTime DateValue
        {
            get
            {
                string valueString = Value.ToUpper();

                if (valueString == "TODAY") return DateTime.Now.Date;

                if (int.TryParse(valueString, out int oneDay))
                {
                    DateTime now = DateTime.Now;
                    return new DateTime(now.Year, now.Month, oneDay);
                }

                try
                {
                    string[] dateParts = Value.Split(new[] { '.' });
                    int day = int.Parse(dateParts[0].Trim());
                    int month = int.Parse(dateParts[1].Trim());
                    int year = int.Parse(dateParts[2].Trim());
                    return new DateTime(year, month, day);
                }
                catch
                {
                    throw new DateNotInCorrectFormat(Value);
                }
            }
        }

        public (int Hours, int Minutes) TimeValue
        {
            get
            {
                try
                {
                    if (int.TryParse(Value, out int hh)) return (hh, 0);

                    string[] timeParts = Value.Split(new[] { ':' });
                    hh = int.Parse(timeParts[0].Trim());
                    int mm = int.Parse(timeParts[1].Trim());
                    return (hh, mm);
                }
                catch
                {
                    throw new TimeNotInCorrectFormat(Value);
                }
            }
        }

        public DatePeriod PeridValue
        {
            get
            {
                string valueAdj = Value.ToUpper();

                if (valueAdj == "TODAY") return new DatePeriod(DateTime.Now.Date, DateTime.Now.Date);

                if (valueAdj == "YEAR")
                {
                    DateTime now = DateTime.Now;
                    return new DatePeriod(new DateTime(now.Year, 1, 1), new DateTime(now.Year, now.Month, now.Day));
                }

                if (valueAdj == "MON" || valueAdj == "MONTH")
                {
                    DateTime now = DateTime.Now;
                    int daysInThisMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    return new DatePeriod(new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, daysInThisMonth));
                }

                if (_dicMonthNames.ContainsKey(valueAdj))
                {
                    int month = _dicMonthNames[valueAdj];
                    DateTime now = DateTime.Now;
                    int daysInThisMonth = DateTime.DaysInMonth(now.Year, month);
                    return new DatePeriod(new DateTime(now.Year, month, 1), new DateTime(now.Year, month, daysInThisMonth));
                }

                if (int.TryParse(valueAdj, out int oneDay))
                {
                    DateTime now = DateTime.Now;
                    return new DatePeriod(new DateTime(now.Year, now.Month, oneDay), new DateTime(now.Year, now.Month, oneDay));
                }

                try
                {
                    string[] periodParts = Value.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime from = DateTime.ParseExact(periodParts[0], "dd.MM.yyyy", null);
                    DateTime to = periodParts.Length > 1 ? DateTime.ParseExact(periodParts[1], "dd.MM.yyyy", null) : from;
                    return new DatePeriod(from, to);
                }
                catch
                {
                    throw new DateNotInCorrectFormat(Value);
                }
            }
        }
    }
}

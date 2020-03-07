using LifeStream108.Libs.Common;
using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Entities;
using System;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class CommandParameterAndValue
    {
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

        public DatePeriod PeridValue
        {
            get
            {
                string valueString = Value.ToUpper();

                if (valueString == "TODAY") return new DatePeriod(DateTime.Now.Date, DateTime.Now.Date);

                if (valueString == "MON" || valueString == "MONTH")
                {
                    DateTime now = DateTime.Now;
                    int daysInThisMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    return new DatePeriod(new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, daysInThisMonth));
                }

                if (int.TryParse(valueString, out int oneDay))
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

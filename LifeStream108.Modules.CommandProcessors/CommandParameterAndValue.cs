using LifeStream108.Libs.Common;
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
                string[] dateParts = Value.Split(new[] { '.' });
                int day = int.Parse(dateParts[0].Trim());
                int month = int.Parse(dateParts[1].Trim());
                int year = int.Parse(dateParts[2].Trim());
                return new DateTime(year, month, day);
            }
        }

        public DatePeriod PeridValue
        {
            get
            {

            }
        }
    }
}

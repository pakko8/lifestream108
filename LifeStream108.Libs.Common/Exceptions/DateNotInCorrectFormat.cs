using System;

namespace LifeStream108.Libs.Common.Exceptions
{
    public class DateNotInCorrectFormat : Exception
    {
        public DateNotInCorrectFormat(string dateValue) : base($"Дата в неверном формате: {dateValue}")
        {
        }
    }
}

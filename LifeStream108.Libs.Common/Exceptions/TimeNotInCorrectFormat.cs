using System;

namespace LifeStream108.Libs.Common.Exceptions
{
    public class TimeNotInCorrectFormat : Exception
    {
        public TimeNotInCorrectFormat(string dateValue) : base($"Время в неверном формате: {dateValue}")
        {
        }
    }
}

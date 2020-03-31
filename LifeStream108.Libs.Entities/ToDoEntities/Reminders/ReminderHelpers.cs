using System;

namespace LifeStream108.Libs.Entities.ToDoEntities.Reminders
{
    internal static class ReminderHelpers
    {
        public static DateTime GetComingSoonReminderTimeForDays(
            DateTime time, int repeatDaysCount, DateTime lastReminderTime, DateTime zeroTime)
        {
            DateTime now = DateTime.Now;
            if (time > now) return time;

            int countTimes = (int)Math.Floor((now - zeroTime).TotalDays / repeatDaysCount);
            DateTime estimatedTime = zeroTime.AddDays(countTimes * repeatDaysCount);
            if (estimatedTime < now) estimatedTime = estimatedTime.AddDays(repeatDaysCount);
            return estimatedTime;
        }
    }
}

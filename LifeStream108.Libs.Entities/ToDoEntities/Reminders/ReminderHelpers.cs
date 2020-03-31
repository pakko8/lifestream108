using System;

namespace LifeStream108.Libs.Entities.ToDoEntities.Reminders
{
    internal static class ReminderHelpers
    {
        public static DateTime GetComingSoonReminderTimeForDays(DateTime time, int repeatDaysCount, DateTime lastReminderTime)
        {
            DateTime now = DateTime.Now;

            if (time > now) return time;

            DateTime zeroTime = lastReminderTime > time ? lastReminderTime : time;
            int countTimes = (int)Math.Floor((now - zeroTime).TotalDays / repeatDaysCount);
            DateTime estimatedTime = zeroTime.AddDays(countTimes * repeatDaysCount);
            if (estimatedTime < now) estimatedTime = estimatedTime.AddDays(repeatDaysCount);
            return estimatedTime;
        }
    }
}

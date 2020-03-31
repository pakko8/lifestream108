using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Common.Grammar;
using NLog;
using System;
using System.Globalization;
using System.Text;

namespace LifeStream108.Libs.Entities.ToDoEntities
{
    /*public class ToDoTaskReminder
    {
        public DateTime GetNextReminderTime(DateTime lastReminderTime)
        {
            DateTime now = DateTime.Now;

            if (Time > now) return Time;

            DateTime zeroTime = lastReminderTime > Time ? lastReminderTime : Time;
            DateTime estimatedTime;
            switch (RepeaterType)
            {
                case ReminderRepeaterType.Once:
                    return Time;
                case ReminderRepeaterType.Day:
                    estimatedTime = new DateTime(now.Year, now.Month, now.Day, Time.Hour, Time.Minute, 0);
                    if (estimatedTime < now) return estimatedTime.AddDays(1);
                    return estimatedTime;
                case ReminderRepeaterType.Week:
                    estimatedTime = new DateTime(now.Year, now.Month, Time.Day, Time.Hour, Time.Minute, 0); // TODO
                    throw NotImplementedRepeaterTypeException;
                case ReminderRepeaterType.Month:
                    estimatedTime = new DateTime(now.Year, now.Month, Time.Day, Time.Hour, Time.Minute, 0);
                    if (estimatedTime < now) return estimatedTime.AddMonths(1);
                    return estimatedTime;
                case ReminderRepeaterType.Year:
                    estimatedTime = new DateTime(now.Year, Time.Month, Time.Day, Time.Hour, Time.Minute, 0);
                    if (estimatedTime < now) return estimatedTime.AddYears(1);
                    return estimatedTime;
                default:
                    throw NotImplementedRepeaterTypeException;
            }
        }

        public bool IsTimeToRemind(DateTime lastReminderTime)
        {
            DateTime now = DateTime.Now;
            DateTime timeToRemind;
            switch (RepeaterType)
            {
                case ReminderRepeaterType.Once:
                    if (lastReminderTime > MinReminderTime) return false;
                    return Time <= now;
                case ReminderRepeaterType.Day:
                    if ((now - Time).TotalHours < 24) return false;
                    return Time <= now;
                case ReminderRepeaterType.Week:
                    if ((now - Time).TotalDays < 7) return false;
                    return Time <= now;
                case ReminderRepeaterType.Month:
                    if ((now - Time).TotalMinutes < 7) return false;
                    return Time <= now;
                case ReminderRepeaterType.Year:
                    timeToRemind = lastReminderTime.AddYears(RepeaterValue);
                    break;
                default:
                    throw NotImplementedRepeaterTypeException;
            }
            throw new NotImplementedException();
        }
    }*/
}

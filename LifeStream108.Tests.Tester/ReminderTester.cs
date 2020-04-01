using LifeStream108.Libs.Entities.ToDoEntities.Reminders;
using System;

namespace LifeStream108.Tests.Tester
{
    internal static class ReminderTester
    {
        public static void Run()
        {
            // Tested when DateTime were 2020-03-30 !!!
            var dailyReminder = Reminder.Create(ReminderRepeaterType.Day, new DateTime(2020, 3, 18), 3);
            DateTime dailyNextTime = dailyReminder.Reminder.GetComingSoonReminderTime(new DateTime(2020, 3, 21)); // Result must be 2020-04-02

            var weeklyReminder = Reminder.Create(ReminderRepeaterType.Week, new DateTime(2020, 2, 27), 3); // Result must be 2020-04-09
            DateTime weeklyNextTime = weeklyReminder.Reminder.GetComingSoonReminderTime(new DateTime(2020, 3, 19));

            var monthlyReminder = Reminder.Create(ReminderRepeaterType.Month, new DateTime(2019, 11, 14), 3); // Result must be 2020-05-14
            DateTime monthlyNextTime = monthlyReminder.Reminder.GetComingSoonReminderTime(DateTime.MinValue);

            var annuallyReminder = Reminder.Create(ReminderRepeaterType.Year, new DateTime(2020, 3, 31), 2); // Result must be 2022-03-31
            DateTime annuallyNextTime = annuallyReminder.Reminder.GetComingSoonReminderTime(DateTime.MinValue);
        }
    }
}

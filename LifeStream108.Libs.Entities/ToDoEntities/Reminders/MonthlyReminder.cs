using System;

namespace LifeStream108.Libs.Entities.ToDoEntities.Reminders
{
    public class MonthlyReminder : Reminder
    {
        internal MonthlyReminder()
        {
        }

        internal MonthlyReminder(DateTime time, int repeaterValue) : base(time, repeaterValue)
        {
        }

        public override ReminderRepeaterType RepeaterType => ReminderRepeaterType.Month;

        public override string DeclationPhraseWhenSingleCase => "каждый месяц";

        public override string DeclationWord1 => "месяц";

        public override string DeclationWord2 => "месяца";

        public override string DeclationWord3 => "месяцев";

        public override DateTime GetComingSoonReminderTime(DateTime lastReminderTime)
        {
            DateTime now = DateTime.Now;
            if (Time > now) return Time;

            DateTime zeroTime = GetZeroTime(lastReminderTime);
            int countTimes = (int)Math.Floor((DateTime.Now - zeroTime).TotalDays / 30 / RepeaterValue);
            DateTime estimatedTime = zeroTime.AddMonths(countTimes * RepeaterValue);
            if (estimatedTime < now) estimatedTime = estimatedTime.AddMonths(RepeaterValue);
            return estimatedTime;
        }
    }
}

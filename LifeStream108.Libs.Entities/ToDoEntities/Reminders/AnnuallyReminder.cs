using System;

namespace LifeStream108.Libs.Entities.ToDoEntities.Reminders
{
    public class AnnuallyReminder : Reminder
    {
        internal AnnuallyReminder()
        {
        }

        internal AnnuallyReminder(DateTime time, int repeaterValue) : base(time, repeaterValue)
        {
        }

        public override ReminderRepeaterType RepeaterType => ReminderRepeaterType.Year;

        public override string DeclationPhraseWhenSingleCase => "каждый год";

        public override string DeclationWord1 => "год";

        public override string DeclationWord2 => "года";

        public override string DeclationWord3 => "лет";

        public override DateTime GetComingSoonReminderTime(DateTime lastReminderTime)
        {
            DateTime now = DateTime.Now;
            if (Time > now) return Time;

            DateTime zeroTime = GetZeroTime(lastReminderTime);
            int countTimes = (int)Math.Floor((DateTime.Now - zeroTime).TotalDays / 365 / RepeaterValue);
            //if (countTimes == 0) countTimes = 1;
            DateTime estimatedTime = zeroTime.AddYears(countTimes * RepeaterValue);
            if (estimatedTime < now) estimatedTime = estimatedTime.AddYears(RepeaterValue);
            return estimatedTime;
        }
    }
}

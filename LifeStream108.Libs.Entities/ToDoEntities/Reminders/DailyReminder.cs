using System;

namespace LifeStream108.Libs.Entities.ToDoEntities.Reminders
{
    public class DailyReminder : Reminder
    {
        internal DailyReminder()
        {
        }

        internal DailyReminder(DateTime time, int repeaterValue) : base(time, repeaterValue)
        {
        }

        public override ReminderRepeaterType RepeaterType => ReminderRepeaterType.Day;

        public override string DeclationPhraseWhenSingleCase => "каждый день";

        public override string DeclationWord1 => "день";

        public override string DeclationWord2 => "дня";

        public override string DeclationWord3 => "дней";

        public override DateTime GetComingSoonReminderTime(DateTime lastReminderTime)
        {
            return ReminderHelpers.GetComingSoonReminderTimeForDays(Time, RepeaterValue, lastReminderTime);
        }
    }
}

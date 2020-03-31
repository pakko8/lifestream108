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
            throw new NotImplementedException();
        }
    }
}

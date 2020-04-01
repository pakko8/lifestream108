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

        protected override int CoefForCalcDiffBetweenDates => 30;

        protected override DateTime AddValueToGetComingSoonReminderTime(DateTime time, int value)
        {
            return time.AddMonths(value);
        }
    }
}

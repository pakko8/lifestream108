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

        protected override int CoefForCalcDiffBetweenDates => 365;

        protected override DateTime AddValueToGetComingSoonReminderTime(DateTime time, int value)
        {
            return time.AddYears(value);
        }
    }
}

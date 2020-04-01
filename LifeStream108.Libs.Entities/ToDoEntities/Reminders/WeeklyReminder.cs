using System;

namespace LifeStream108.Libs.Entities.ToDoEntities.Reminders
{
    public class WeeklyReminder : Reminder
    {
        internal WeeklyReminder()
        {
        }

        internal WeeklyReminder(DateTime time, int repeaterValue) : base(time, repeaterValue)
        {
        }

        public override ReminderRepeaterType RepeaterType => ReminderRepeaterType.Week;

        public override string DeclationPhraseWhenSingleCase => "каждую неделю";

        public override string DeclationWord1 => "неделя";

        public override string DeclationWord2 => "недели";

        public override string DeclationWord3 => "неделей";

        protected override int CoefForCalcDiffBetweenDates => 7;

        protected override DateTime AddValueToGetComingSoonReminderTime(DateTime time, int value)
        {
            return time.AddDays(value * CoefForCalcDiffBetweenDates);
        }
    }
}

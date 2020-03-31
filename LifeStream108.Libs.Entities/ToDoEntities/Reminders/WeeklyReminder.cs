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

        public override DateTime GetComingSoonReminderTime(DateTime lastReminderTime)
        {
            return ReminderHelpers.GetComingSoonReminderTimeForDays(Time, RepeaterValue * 7, lastReminderTime);
        }
    }
}

using System;

namespace LifeStream108.Libs.Entities.ToDoEntities.Reminders
{
    public class OnceReminder : Reminder
    {
        internal OnceReminder()
        {
        }

        internal OnceReminder(DateTime time, int repeaterValue) : base(time, repeaterValue)
        {
        }

        public override ReminderRepeaterType RepeaterType => ReminderRepeaterType.Once;

        public override bool IsRepetitive => false;

        public override string DeclationPhraseWhenSingleCase => "";

        public override string DeclationWord1 => "";

        public override string DeclationWord2 => "";

        public override string DeclationWord3 => "";

        public override bool IsTimeToRemind(DateTime lastReminderTime)
        {
            if (lastReminderTime > ZeroReminderTime) return false;
            return base.IsTimeToRemind(lastReminderTime);
        }

        public override DateTime GetComingSoonReminderTime(DateTime lastReminderTime)
        {
            return Time;
        }

        public override string FormatReminderForUser(DateTime lastReminderTime)
        {
            return $"{Time:dd.MM.yyyy HH:mm}";
        }

        protected override int CoefForCalcDiffBetweenDates => throw new NotImplementedException("No need for this reminder");

        protected override DateTime AddValueToGetComingSoonReminderTime(DateTime time, int value)
        {
            throw new NotImplementedException("No need for this reminder");
        }
    }
}

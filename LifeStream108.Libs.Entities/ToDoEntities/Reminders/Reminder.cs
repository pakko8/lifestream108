using LifeStream108.Libs.Common.Grammar;
using NLog;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LifeStream108.Libs.Entities.ToDoEntities.Reminders
{
    public abstract class Reminder
    {
        protected const string UserTimeFormat = "dd.MM.yyyy HH:mm";
        private const string SystemTimeFormat = "yyyy-MM-dd HH:mm";

        /// <summary>
        /// Пороговое время напоминаний. Предполагается, что ранее этого времени система не работала
        /// </summary>
        protected readonly DateTime ZeroReminderTime = new DateTime(2020, 1, 1);

        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected Reminder()
        {
        }

        protected Reminder(DateTime time, int repeaterValue)
        {
            Time = time;
            RepeaterValue = repeaterValue;
        }

        public DateTime Time { get; private set; } = DateTime.MinValue;

        public int RepeaterValue { get; private set; } = 0;

        public abstract ReminderRepeaterType RepeaterType { get; }

        public abstract string DeclationPhraseWhenSingleCase { get; }

        public abstract string DeclationWord1 { get; }

        public abstract string DeclationWord2 { get; }

        public abstract string DeclationWord3 { get; }

        public abstract DateTime GetComingSoonReminderTime(DateTime lastReminderTime);

        public bool IsTimeToRemind(DateTime lastReminderTime)
        {
            throw new NotImplementedException(); // TODO
            /*DateTime reminderTime = GetComingSoonReminderTime(lastReminderTime);
            return reminderTime >= DateTime.Now;*/
        }

        public string UserFormattedTime => Time.ToString(UserTimeFormat);

        public static (Reminder Reminder, string Error) Create(ReminderRepeaterType repeaterType, DateTime time, int repeaterValue)
        {
            switch (repeaterType)
            {
                case ReminderRepeaterType.Once:
                    return (new OnceReminder(time, repeaterValue), "");
                case ReminderRepeaterType.Day:
                    return (new DailyReminder(time, repeaterValue), "");
                case ReminderRepeaterType.Week:
                    return (new WeeklyReminder(time, repeaterValue), "");
                case ReminderRepeaterType.Month:
                    return (new MonthlyReminder(time, repeaterValue), "");
                case ReminderRepeaterType.Year:
                    return (new AnnuallyReminder(time, repeaterValue), "");
                default:
                    return (null, "Обнаружен нереализованный тип повторения задачи");
            }
        }

        public static (Reminder Reminder, string Error) Create(string reminderFormat)
        {
            if (string.IsNullOrEmpty(reminderFormat)) return (new OnceReminder(), "");

            string[] formatParts = reminderFormat.Split(new[] { '{' });
            DateTime time = DateTime.ParseExact(formatParts[0], SystemTimeFormat, null);

            string repeatTypeString = formatParts[1].Replace("}", "");
            string[] repeaterParts = repeatTypeString.Split(new[] { ':' });
            int repeaterValue = repeaterParts.Length > 1 ? int.Parse(repeaterParts[1]) : 0;
            ReminderRepeaterType repeaterType = (ReminderRepeaterType)Enum.Parse(typeof(ReminderRepeaterType), repeaterParts[0], true);
            return Create(repeaterType, time, repeaterValue);
        }

        public static (Reminder Reminder, string Error) Create(
            string timeValue, string repeaterValueString, string repeaterTypeString)
        {
            if (string.IsNullOrEmpty(repeaterValueString)) repeaterValueString = "0";
            bool parseFlag = int.TryParse(repeaterValueString, out int repeaterValue);
            if (!parseFlag) return (null, $"Количество повторений '{repeaterValueString}' должно быть целым числом");

            ReminderRepeaterType repeaterType;
            if (string.IsNullOrEmpty(repeaterTypeString))
            {
                repeaterType = ReminderRepeaterType.Once;
            }
            else
            {
                parseFlag = Enum.TryParse(repeaterTypeString, out repeaterType);
                if (!parseFlag) return (null, "Не удалось распознать тип повторения: " + repeaterTypeString);
            }

            if (repeaterValue > 0 && repeaterType == ReminderRepeaterType.Once)
                return (null, $"Если указано количество повторений, то тип повторения не должен быть '{ReminderRepeaterType.Once.ToString()}'");

            if (repeaterType != ReminderRepeaterType.Once && repeaterValue == 0)
                return (null, $"Если указан тип повторения, то также должно быть указано количество повторений");

            parseFlag = DateTime.TryParseExact(
                timeValue, UserTimeFormat, null, DateTimeStyles.None, out DateTime time);
            if (!parseFlag) return (null, "Время в неверном формате. Верный формат: " + UserTimeFormat);

            return Create(repeaterType, time, repeaterValue);
        }

        public string ReminderFormat
        {
            get
            {
                StringBuilder sbFormat = new StringBuilder(Time.ToString(SystemTimeFormat, null));
                sbFormat.Append('{');
                sbFormat.Append(RepeaterType.ToString().ToLower());
                if (RepeaterType != ReminderRepeaterType.Once) sbFormat.Append(":" + RepeaterValue);
                sbFormat.Append('}');
                return sbFormat.ToString();
            }
        }

        public virtual string FormatReminderForUser(DateTime lastReminderTime)
        {
            return GetComingSoonReminderTime(lastReminderTime).ToString(UserTimeFormat) + " " +
                (RepeaterValue > 1
                ? DeclineWordEach(RepeaterValue) + Declanations.DeclineByNumeral(
                    RepeaterValue, DeclationWord1, DeclationWord2, DeclationWord3)
                : DeclationPhraseWhenSingleCase);
        }

        protected static string DeclineWordEach(int count)
        {
            return $"{Declanations.DeclineByNumeral(count, "каждый", "каждые", "каждые")} {count} ";
        }

        /// <summary>
        /// Время, от которого считать, когда снова должен сработать будильник
        /// </summary>
        protected DateTime GetZeroTime(DateTime lastReminderTime)
        {
            return lastReminderTime > Time ? lastReminderTime : Time;
        }
    }
}

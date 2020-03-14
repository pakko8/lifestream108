using System;
using System.Globalization;
using System.Text;

namespace LifeStream108.Libs.Entities.ToDoEntities
{
    /// <summary>
    /// Remind at {Time}. Also reminder can be repeated every {RepeaterValue} {RepeaterType}
    /// </summary>
    public class ToDoTaskReminder
    {
        private const string UserTimeFormat = "dd.MM.yyyy HH:mm";
        private const string SystemTimeFormat = "yyyy-MM-dd HH:mm";

        public DateTime Time { get; private set; }

        public int RepeaterValue { get; private set; }

        public ReminderRepeaterType RepeaterType { get; private set; }

        public string ReminderFormat
        {
            get
            {
                StringBuilder sbFormat = new StringBuilder(Time.ToString(SystemTimeFormat, null));
                sbFormat.Append('{');
                sbFormat.Append(RepeaterType.ToString().ToLower());
                if (RepeaterType != ReminderRepeaterType.Once) sbFormat.Append(':' + RepeaterValue);
                sbFormat.Append('}');
                return sbFormat.ToString();
            }
        }

        public void Load(string reminderFormat)
        {
            string[] formatParts = reminderFormat.Split(new[] { '{' });
            Time = DateTime.ParseExact(formatParts[0], SystemTimeFormat, null);

            string repeatTypeString = formatParts[1].Replace("}", "");
            string[] repeaterParts = repeatTypeString.Split(new[] { ':' });
            RepeaterType = (ReminderRepeaterType)Enum.Parse(typeof(ReminderRepeaterType), repeaterParts[0], true);
            RepeaterValue = repeaterParts.Length > 1 ? int.Parse(repeaterParts[1]) : 0;
        }

        public string Load(string timeValue, int repeaterValue, ReminderRepeaterType repeaterType)
        {
            bool parseTimeSuccess = DateTime.TryParseExact(
                timeValue, UserTimeFormat, null, DateTimeStyles.None, out DateTime time);
            if (!parseTimeSuccess) return "Время в неверном формате. Верный формат: " + UserTimeFormat;

            Time = time;
            RepeaterValue = repeaterValue;
            RepeaterType = repeaterType;
            return null;
        }
    }

    public enum ReminderRepeaterType
    {
        Once,

        Day,

        Week,

        Year
    }
}

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

        public DateTime Time { get; private set; } = DateTime.MinValue;

        public string UserFormattedTime => Time.ToString(UserTimeFormat);

        public int RepeaterValue { get; private set; } = 0;

        public ReminderRepeaterType RepeaterType { get; private set; } = ReminderRepeaterType.Once;

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

        public void Load(string reminderFormat)
        {
            if (string.IsNullOrEmpty(reminderFormat)) return;

            string[] formatParts = reminderFormat.Split(new[] { '{' });
            Time = DateTime.ParseExact(formatParts[0], SystemTimeFormat, null);

            string repeatTypeString = formatParts[1].Replace("}", "");
            string[] repeaterParts = repeatTypeString.Split(new[] { ':' });
            RepeaterType = (ReminderRepeaterType)Enum.Parse(typeof(ReminderRepeaterType), repeaterParts[0], true);
            RepeaterValue = repeaterParts.Length > 1 ? int.Parse(repeaterParts[1]) : 0;
        }

        public void Load(DateTime dateAndTime, int repeaterValue, ReminderRepeaterType repeaterType)
        {
            Time = dateAndTime;
            RepeaterValue = repeaterValue;
            RepeaterType = repeaterType;
        }

        public string Load(string timeValue, string repeaterValueString, string repeaterTypeString)
        {
            if (string.IsNullOrEmpty(repeaterValueString)) repeaterValueString = "0";
            bool parseFlag = int.TryParse(repeaterValueString, out int repeaterValue);
            if (!parseFlag) return $"Количество повторений '{repeaterValueString}' должно быть целым числом";

            ReminderRepeaterType repeaterType;
            if (string.IsNullOrEmpty(repeaterTypeString))
            {
                repeaterType = ReminderRepeaterType.Once;
            }
            else
            {
                parseFlag = Enum.TryParse(repeaterTypeString, out repeaterType);
                if (!parseFlag) return "Не удалось распознать тип повторения: " + repeaterTypeString;
            }

            if (repeaterValue > 0 && repeaterType == ReminderRepeaterType.Once)
                return $"Если указано количество повторений, то тип повторения не должен быть '{ReminderRepeaterType.Once.ToString()}'";

            if (repeaterType != ReminderRepeaterType.Once && repeaterValue == 0)
                return $"Если указан тип повторения, то также должно быть указано количество повторений";

            parseFlag = DateTime.TryParseExact(
                timeValue, UserTimeFormat, null, DateTimeStyles.None, out DateTime time);
            if (!parseFlag) return "Время в неверном формате. Верный формат: " + UserTimeFormat;

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

        Month,

        Year
    }
}

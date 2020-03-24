using LifeStream108.Libs.Common.Grammar;
using LifeStream108.Libs.Entities.ToDoEntities;
using NLog;
using System.Text;

namespace LifeStream108.Modules.CommandToDoProcessors
{
    internal static class ProcessorHelpers
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static string PrintTaskReminder(string reminderSettings)
        {
            ToDoTaskReminder reminder = new ToDoTaskReminder();
            reminder.Load(reminderSettings);

            StringBuilder sbResultInfo = new StringBuilder(reminder.Time.ToString("dd.MM.yyyy HH:mm"));
            if (reminder.RepeaterValue > 0 && reminder.RepeaterType != ReminderRepeaterType.Once)
            {
                sbResultInfo.Append(", ");
                switch (reminder.RepeaterType)
                {
                    case ReminderRepeaterType.Day when reminder.RepeaterValue == 1:
                        sbResultInfo.Append("каждый день");
                        break;
                    case ReminderRepeaterType.Day when reminder.RepeaterValue > 1:
                        sbResultInfo.Append(DeclineWordEach(reminder.RepeaterValue) + " " +
                            Declanations.DeclineByNumeral(reminder.RepeaterValue, "день", "дня", "дней"));
                        break;
                    case ReminderRepeaterType.Week when reminder.RepeaterValue == 1:
                        sbResultInfo.Append("каждую неделю");
                        break;
                    case ReminderRepeaterType.Week when reminder.RepeaterValue > 1:
                        sbResultInfo.Append(DeclineWordEach(reminder.RepeaterValue) + " " +
                            Declanations.DeclineByNumeral(reminder.RepeaterValue, "неделя", "недели", "неделей"));
                        break;
                    case ReminderRepeaterType.Month when reminder.RepeaterValue == 1:
                        sbResultInfo.Append("каждый месяц");
                        break;
                    case ReminderRepeaterType.Month when reminder.RepeaterValue > 1:
                        sbResultInfo.Append(DeclineWordEach(reminder.RepeaterValue) + " " +
                            Declanations.DeclineByNumeral(reminder.RepeaterValue, "месяц", "месяца", "месяцев"));
                        break;
                    case ReminderRepeaterType.Year when reminder.RepeaterValue == 1:
                        sbResultInfo.Append("каждый год");
                        break;
                    case ReminderRepeaterType.Year when reminder.RepeaterValue > 1:
                        sbResultInfo.Append(DeclineWordEach(reminder.RepeaterValue) + " " +
                            Declanations.DeclineByNumeral(reminder.RepeaterValue, "год", "года", "лет"));
                        break;
                    default:
                        Logger.Warn($"No options to print reminder for '{reminderSettings}'");
                        break;
                }
            }
            return sbResultInfo.ToString();
        }

        private static string DeclineWordEach(int count)
        {
            return Declanations.DeclineByNumeral(count, "каждый", "каждые", "каждые");
        }
    }
}

using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Common.Grammar;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.CommandProcessors;
using NLog;
using System;
using System.Linq;
using System.Text;

namespace LifeStream108.Modules.CommandToDoProcessors
{
    internal static class ProcessorHelpers
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ToDoTaskReminder ReadReminder(CommandParameterAndValue[] parameters)
        {
            ToDoTaskReminder reminder = null;
            CommandParameterAndValue reminderDateParameter = parameters.FirstOrDefault(
               n => n.Parameter.ParameterCode == CommandParameterCode.Date);
            if (reminderDateParameter != null)
            {
                DateTime reminderDate = reminderDateParameter.DateValue;

                CommandParameterAndValue reminderTimeParameter = parameters.FirstOrDefault(
                    n => n.Parameter.ParameterCode == CommandParameterCode.Time);
                if (reminderTimeParameter == null) throw new LifeStream108Exception(ErrorType.WrangReminderSettings,
                    "Reminder date is set but time is absent",
                    "Для задачи указана дата напоминания, но не указано время", "");

                (int Hours, int Minutes) reminderTime = reminderTimeParameter.TimeValue;

                CommandParameterAndValue repeaterParameter = parameters.FirstOrDefault(
                    n => n.Parameter.ParameterCode == CommandParameterCode.ToDoTaskRepeater);
                int repeaterValue = 0;
                ReminderRepeaterType repeaterType = ReminderRepeaterType.Once;
                if (repeaterParameter != null)
                {
                    string[] repeaterParts = repeaterParameter.Value.Split(new[] { ',' });
                    repeaterValue = int.Parse(repeaterParts[1].Trim());
                    switch (repeaterParts[0].ToUpper().Trim())
                    {
                        case "DAY":
                        case "ДЕНЬ":
                            repeaterType = ReminderRepeaterType.Day;
                            break;
                        case "WEEK":
                        case "НЕДЕЛЯ":
                            repeaterType = ReminderRepeaterType.Week;
                            break;
                        case "MONTH":
                        case "MON":
                        case "МЕСЯЦ":
                            repeaterType = ReminderRepeaterType.Month;
                            break;
                        case "YEAR":
                        case "ГОД":
                            repeaterType = ReminderRepeaterType.Year;
                            break;
                        default:
                            repeaterType = ReminderRepeaterType.Once;
                            break;
                    }
                }
                reminder = new ToDoTaskReminder();
                reminder.Load(
                    new DateTime(reminderDate.Year, reminderDate.Month, reminderDate.Day,
                    reminderTime.Hours, reminderTime.Minutes, 0),
                    repeaterValue, repeaterType);
            }
            return reminder;
        }

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
                        sbResultInfo.Append(DeclineWordEach(reminder.RepeaterValue) +
                            Declanations.DeclineByNumeral(reminder.RepeaterValue, "день", "дня", "дней"));
                        break;
                    case ReminderRepeaterType.Week when reminder.RepeaterValue == 1:
                        sbResultInfo.Append("каждую неделю");
                        break;
                    case ReminderRepeaterType.Week when reminder.RepeaterValue > 1:
                        sbResultInfo.Append(DeclineWordEach(reminder.RepeaterValue) +
                            Declanations.DeclineByNumeral(reminder.RepeaterValue, "неделя", "недели", "неделей"));
                        break;
                    case ReminderRepeaterType.Month when reminder.RepeaterValue == 1:
                        sbResultInfo.Append("каждый месяц");
                        break;
                    case ReminderRepeaterType.Month when reminder.RepeaterValue > 1:
                        sbResultInfo.Append(DeclineWordEach(reminder.RepeaterValue) +
                            Declanations.DeclineByNumeral(reminder.RepeaterValue, "месяц", "месяца", "месяцев"));
                        break;
                    case ReminderRepeaterType.Year when reminder.RepeaterValue == 1:
                        sbResultInfo.Append("каждый год");
                        break;
                    case ReminderRepeaterType.Year when reminder.RepeaterValue > 1:
                        sbResultInfo.Append(DeclineWordEach(reminder.RepeaterValue) +
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
            return $"{Declanations.DeclineByNumeral(count, "каждый", "каждые", "каждые")} {count} ";
        }
    }
}

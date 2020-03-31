using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.ToDoEntities.Reminders;
using LifeStream108.Modules.CommandProcessors;
using System;
using System.Linq;

namespace LifeStream108.Modules.CommandToDoProcessors
{
    internal static class ProcessorHelpers
    {
        public static Reminder ReadReminder(CommandParameterAndValue[] parameters)
        {
            CommandParameterAndValue reminderDateParameter = parameters.FirstOrDefault(
               n => n.Parameter.ParameterCode == CommandParameterCode.Date);
            if (reminderDateParameter == null) return null;

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
            var createReminderResult = Reminder.Create(repeaterType,
                new DateTime(reminderDate.Year, reminderDate.Month, reminderDate.Day,
                    reminderTime.Hours, reminderTime.Minutes, 0),
                repeaterValue);
            if (!string.IsNullOrEmpty(createReminderResult.Error))
                throw new LifeStream108Exception(ErrorType.WrangReminderSettings, "", createReminderResult.Error, "");

            return createReminderResult.Reminder;
        }
    }
}

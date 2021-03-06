﻿using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.Entities.ToDoEntities.Reminders;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.ToDoListManagement;
using System.Linq;
using System.Text;

namespace LifeStream108.Modules.CommandToDoProcessors.TaskProcessors
{
    public class TaskInfoProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue taskCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.ToDoTaskCode);

            var findTaskResult = session.GetNumberDataValue(taskCodeParameter.Value);
            if (!string.IsNullOrEmpty(findTaskResult.Error))
                return ExecuteCommandResult.CreateErrorObject(findTaskResult.Error);

            ToDoTask task = ToDoTaskManager.GetTask((int)findTaskResult.Value);
            Logger.Info("Show info for task " + task.Id);

            ToDoList list = ToDoListManager.GetList(task.ListId);
            StringBuilder sbTaskInfo = new StringBuilder($"Находится в списке: <b>{list.Name}</b>\r\n");
            sbTaskInfo.Append($"<b>{TelegramUtils.RemoveUnsafeSigns(task.Title)}</b>\r\n");
            if (!string.IsNullOrEmpty(task.ReminderSettings))
            {
                var createReminderResult = Reminder.Create(task.ReminderSettings);
                if (!string.IsNullOrEmpty(createReminderResult.Error))
                    return ExecuteCommandResult.CreateErrorObject(createReminderResult.Error);

                sbTaskInfo.Append($"    <i>Напоминание</i>: {createReminderResult.Reminder.FormatReminderForUser(task.ReminderLastTime)}\r\n\r\n");
            }
            sbTaskInfo.Append(task.Note);

            return ExecuteCommandResult.CreateSuccessObject(sbTaskInfo.ToString());
        }
    }
}

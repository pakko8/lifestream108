using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.ToDoListManagement.Managers;
using System.Linq;

namespace LifeStream108.Modules.CommandToDoProcessors.TaskProcessors
{
    public class SetTaskReminderProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue taskCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.ToDoTaskCode);

            var findTaskResult = session.GetNumberDataValue(taskCodeParameter.Value);
            if (!string.IsNullOrEmpty(findTaskResult.Error))
                return ExecuteCommandResult.CreateErrorObject(findTaskResult.Error);

            ToDoTask task = ToDoTaskManager.GetTask((int)findTaskResult.Value);
            Logger.Info($"Current title of task {task.Id}: <{task.Title}>");

            ToDoTaskReminder reminder = ProcessorHelpers.ReadReminder(commandParameters);

            task.ReminderSettings = reminder != null ? reminder.ReminderFormat : "";

            string reminderText = ProcessorHelpers.PrintTaskReminder(task.ReminderSettings);
            return ExecuteCommandResult.CreateSuccessObject(
                $"Для задачи '{task.Title}' установлено напоминание: {reminderText}");
        }
    }
}

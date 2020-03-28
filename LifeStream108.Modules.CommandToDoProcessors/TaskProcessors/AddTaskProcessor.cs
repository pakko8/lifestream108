using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.ToDoListManagement.Managers;
using System.Linq;

namespace LifeStream108.Modules.CommandToDoProcessors.TaskProcessors
{
    public class AddTaskProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue listCodeParameter = commandParameters.FirstOrDefault(
               n => n.Parameter.ParameterCode == CommandParameterCode.ToDoListCode);

            ToDoList list = ToDoListManager.GetListByCode(listCodeParameter.IntValue, session.UserId);
            if (list == null) return ExecuteCommandResult.CreateErrorObject(
                "Не найден лист с кодом " + listCodeParameter.IntValue);

            CommandParameterAndValue titleParameter = commandParameters.FirstOrDefault(
              n => n.Parameter.ParameterCode == CommandParameterCode.ToDoTaskTitle);

            ToDoTask task = ToDoTaskManager.GetTaskByTitle(titleParameter.Value, list.Id);
            if (task != null) return ExecuteCommandResult.CreateErrorObject(
                $"Задача с заголовком '{titleParameter.Value}' уже существует");

            ToDoTaskReminder reminder = ProcessorHelpers.ReadReminder(commandParameters);

            task = new ToDoTask();
            task.UserId = session.UserId;
            task.ListId = list.Id;
            task.Title = titleParameter.Value.Trim();
            task.ReminderSettings = reminder != null ? reminder.ReminderFormat : "";
            ToDoTaskManager.AddTask(task);

            string reminderText = !string.IsNullOrEmpty(task.ReminderSettings)
                ? " Напоминание: " + reminder.FormatReminderForUser()
                : "";
            ExecuteCommandResult executeResult = ExecuteCommandResult.CreateSuccessObject(
                $"Задача '{task.Title}' добавлена.{reminderText}");
            executeResult.AddSessionValue(task.Id);
            return executeResult;
        }
    }
}

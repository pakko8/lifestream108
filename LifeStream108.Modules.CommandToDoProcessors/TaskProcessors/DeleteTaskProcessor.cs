using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.ToDoListManagement.Managers;
using System;
using System.Linq;

namespace LifeStream108.Modules.CommandToDoProcessors.TaskProcessors
{
    public class DeleteTaskProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue taskCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.ToDoTaskCode);

            var findTaskResult = session.GetNumberDataValue(taskCodeParameter.Value);
            if (!string.IsNullOrEmpty(findTaskResult.Error))
                return ExecuteCommandResult.CreateErrorObject(findTaskResult.Error);

            ToDoTask task = ToDoTaskManager.GetTask((int)findTaskResult.Value);
            Logger.Info("Delete task " + task.Id);

            task.Status = ToDoTaskStatus.Deleted;
            task.StatusUpdateTime = DateTime.Now;
            ToDoTaskManager.UpdateTask(task);

            return ExecuteCommandResult.CreateSuccessObject($"Задача '{task.Title}' деактивирована");
        }
    }
}

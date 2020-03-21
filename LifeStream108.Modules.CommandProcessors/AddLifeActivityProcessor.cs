using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using System.Linq;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class AddLifeActivityProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityNameParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityName);
            LifeActivity newActivity = LifeActivityManager.GetActivityByName(activityNameParameter.Value, session.UserId);
            if (newActivity == null)
            {
                newActivity = new LifeActivity
                {
                    UserId = session.UserId,
                    Name = activityNameParameter.Value
                };
                LifeActivityManager.AddActivity(newActivity);

                return ExecuteCommandResult.CreateSuccessObject(
                    $"Деятельность \"{newActivity.NameForUser}\" добавлена.");
            }
            else if (!newActivity.Active)
            {
                newActivity.Active = true;
                LifeActivityManager.UpdateActivity(newActivity);
                return ExecuteCommandResult.CreateSuccessObject(
                    $"Деятельность \"{newActivity.NameForUser}\" восстановлена после удаления.");
            }
            else return ExecuteCommandResult.CreateErrorObject(
                $"Деятельность \"{newActivity.NameForUser}\" уже существует.");
        }
    }
}

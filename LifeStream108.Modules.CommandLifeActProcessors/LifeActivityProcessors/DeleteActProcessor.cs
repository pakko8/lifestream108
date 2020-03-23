using System.Linq;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeActivityProcessors
{
    public class DeleteActProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);

            LifeActivity activity = LifeActivityManager.GetActivityByUserCode(activityCodeParameter.IntValue, session.UserId);
            if (activity == null)
                return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityCodeParameter.IntValue} не найдена");

            activity.Active = false;
            LifeActivityManager.UpdateActivity(activity);

            return ExecuteCommandResult.CreateSuccessObject("Деятельность деактивирована");
        }
    }
}

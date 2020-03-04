using System.Linq;
using LifeStream108.Libs.Entities;
using LifeStream108.Modules.LifeActivityManagement.Managers;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class RenameLifeActivityProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);
            CommandParameterAndValue activityNameParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityName);

            LifeActivity activity = LifeActivityManager.GetActivityByUserCode(activityCodeParameter.IntValue, session.UserId);
            if (activity == null)
                return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityCodeParameter.IntValue} не найдена");

            string oldName = activity.NameForUser;
            activity.Name = activityNameParameter.Value;
            LifeActivityManager.UpdateActivity(activity);

            return ExecuteCommandResult.CreateSuccessObject($"Название деятельности \"{oldName}\" изменено на \"{activity.NameForUser}\"");
        }
    }
}

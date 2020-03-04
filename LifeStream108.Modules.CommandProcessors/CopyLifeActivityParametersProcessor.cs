using System.Linq;
using LifeStream108.Libs.Entities;
using LifeStream108.Modules.LifeActivityManagement.Managers;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class CopyLifeActivityParametersProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityCodeFromParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);
            CommandParameterAndValue activityCodeToParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode2);

            var actWithParamsFrom =
                LifeActivityManager.GetActivityAndParamsByUserCode(activityCodeFromParameter.IntValue, session.UserId);
            if (actWithParamsFrom.Activity == null)
                return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityCodeFromParameter.IntValue} не найдена");
            if (actWithParamsFrom.Parameters == null || actWithParamsFrom.Parameters.Length == 0)
                return ExecuteCommandResult.CreateErrorObject($"У деятельности \"{actWithParamsFrom.Activity.NameForUser}\" нет параметров");

            var actWithParamsTo =
                LifeActivityManager.GetActivityAndParamsByUserCode(activityCodeToParameter.IntValue, session.UserId);
            if (actWithParamsTo.Activity == null)
                return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityCodeToParameter.IntValue} не найдена");
            if (actWithParamsTo.Parameters != null && actWithParamsTo.Parameters.Length > 0)
                return ExecuteCommandResult.CreateErrorObject($"У деятельности \"{actWithParamsTo.Activity.NameForUser}\" уже есть параметры. " +
                    "Копировать параметры можно в деятельность без параметров");

            LifeActivityParameter[] copiedParameters = actWithParamsFrom.Parameters.Where (n => n.Active).ToArray();
            foreach (LifeActivityParameter parameter in copiedParameters)
            {
                parameter.ActivityId = actWithParamsTo.Activity.Id;
            }
            LifeActivityParameterManager.AddParameters(copiedParameters);

            return ExecuteCommandResult.CreateSuccessObject(
                $"Параметры \"{actWithParamsFrom.Activity.NameForUser}\" скопированы в \"{actWithParamsTo.Activity.NameForUser}\"");
        }
    }
}

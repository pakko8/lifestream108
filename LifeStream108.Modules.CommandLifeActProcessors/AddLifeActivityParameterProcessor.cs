using System.Linq;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors
{
    public class AddLifeActivityParameterProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);
            CommandParameterAndValue paramNameParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamName);
            CommandParameterAndValue measureParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamMeasureName);
            CommandParameterAndValue dataTypeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamDataType);
            CommandParameterAndValue functionParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamFunc);

            var actWithParams = LifeActivityManager.GetActivityAndParamsByUserCode(activityCodeParameter.IntValue, session.UserId);
            if (actWithParams.Activity == null)
                return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityCodeParameter.IntValue} не найдена");

            LifeActivityParameter parameter = LifeActivityParameterManager.GetParameterByName(
                paramNameParameter.Value, actWithParams.Activity.Id, session.UserId);
            if (parameter != null)
            {
                if (!parameter.Active)
                {
                    parameter.Active = true;
                    LifeActivityParameterManager.UpdateParameter(parameter);
                    return ExecuteCommandResult.CreateSuccessObject($"Параметр с кодом {parameter.UserCode} восстановлен");
                }
                else
                {
                    return ExecuteCommandResult.CreateErrorObject("Деятельность с таким наименованием уже существует");
                }
            }

            int nextSortOrder = actWithParams.Parameters.Length > 0 ? actWithParams.Parameters.Max(n => n.SortOrder) + 1 : 1;
            parameter = new LifeActivityParameter
            {
                UserId = session.UserId,
                SortOrder = nextSortOrder,
                ActivityId = actWithParams.Activity.Id,
                Name = paramNameParameter.Value,
                MeasureId = ProcessorHelpers.GetMeasureId(measureParameter.Value, session.UserId),
                DataType = ProcessorHelpers.GetActivityParameterDataType(dataTypeParameter.Value),
                Fuction = ProcessorHelpers.GetFunction(functionParameter)
            };
            LifeActivityParameterManager.AddParameters(new LifeActivityParameter[] { parameter });

            return ExecuteCommandResult.CreateSuccessObject($"Для \"{actWithParams.Activity.NameForUser}\" добавлен параметр с кодом {parameter.UserCode}");
        }
    }
}

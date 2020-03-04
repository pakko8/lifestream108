using System.Linq;
using LifeStream108.Libs.Entities;
using LifeStream108.Modules.LifeActivityManagement.Managers;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class DeleteLifeActivityParameterProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue paramCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamCode);

            LifeActivityParameter parameter = LifeActivityParameterManager.GetParameterByCode(paramCodeParameter.IntValue, session.UserId);
            if (parameter == null)
                return ExecuteCommandResult.CreateErrorObject($"Параметр с кодом {paramCodeParameter.IntValue} не найден");

            if (!parameter.Active)
                return ExecuteCommandResult.CreateErrorObject($"Параметр с кодом {paramCodeParameter.IntValue} уже деативирован");

            parameter.Active = false;
            LifeActivityParameterManager.UpdateParameter(parameter);

            return ExecuteCommandResult.CreateSuccessObject($"Параметр с кодом {parameter.UserCode} деактивирован");
        }
    }
}

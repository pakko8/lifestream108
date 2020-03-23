using System.Linq;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.ActivityParameterProcessors
{
    public class DeleteParamProcessor : BaseCommandProcessor
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

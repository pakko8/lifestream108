﻿using System.Linq;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.LifeActivityManagement;

namespace LifeStream108.Modules.CommandLifeActProcessors
{
    public class EditParamProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue paramCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamCode);
            CommandParameterAndValue paramNameParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamName);
            CommandParameterAndValue measureParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamMeasureName);
            CommandParameterAndValue dataTypeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamDataType);
            CommandParameterAndValue functionParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamFunc);

            LifeActivityParameter parameter = LifeActivityParameterManager.GetParameterByCode(paramCodeParameter.IntValue, session.UserId);
            if (parameter == null)
                return ExecuteCommandResult.CreateErrorObject($"Параметр с кодом {paramCodeParameter.IntValue} не найден");

            parameter.Name = paramNameParameter.Value;
            parameter.MeasureId = ProcessorHelpers.GetMeasureId(measureParameter.Value, session.UserId);
            parameter.DataType = ProcessorHelpers.GetActivityParameterDataType(dataTypeParameter.Value);
            parameter.Fuction = ProcessorHelpers.GetFunction(functionParameter);
            LifeActivityParameterManager.UpdateParameter(parameter);

            return ExecuteCommandResult.CreateSuccessObject($"Параметр с кодом {parameter.UserCode} изменён");
        }
    }
}

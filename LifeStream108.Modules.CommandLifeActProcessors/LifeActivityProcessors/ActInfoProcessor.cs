﻿using System.Linq;
using System.Text;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.LifeActivityManagement;
using LifeStream108.Modules.DictionaryManagement;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeActivityProcessors
{
    public class ActInfoProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);
            var actWithParams = LifeActivityManager.GetActivityAndParamsByUserCode(activityCodeParameter.IntValue, session.UserId);
            if (actWithParams.Activity == null)
                return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityCodeParameter.IntValue} не найдена");

            StringBuilder sbResult = new StringBuilder(
                $@"[{actWithParams.Activity.UserCode}] <b>{actWithParams.Activity.Name}</b>
                    {ProcessorHelpers.PrintPeriodicityType(actWithParams.Activity.PeriodType, ", ")}\r\n");
            if (actWithParams.Parameters.Length > 0)
            {
                Measure[] measures = MeasureManager.GetMeasuresForUser(session.UserId);
                foreach (LifeActivityParameter parameter in actWithParams.Parameters.OrderBy(n => n.SortOrder))
                {
                    Measure measure = measures.FirstOrDefault(n => n.Id == parameter.MeasureId);
                    sbResult.Append($"    {parameter.NameForUser}, Единица измерения: {measure.Name}, " +
                        $"Тип: {parameter.DataType.GetDescriptiveName()}" +
                        $"{(!string.IsNullOrEmpty(parameter.Fuction) ? ", Функция: " + parameter.Fuction : "")}\r\n");
                }
            }
            else sbResult.Append("Параметры у этой деятельности отсутствуют");

            return ExecuteCommandResult.CreateSuccessObject(sbResult.ToString());
        }
    }
}

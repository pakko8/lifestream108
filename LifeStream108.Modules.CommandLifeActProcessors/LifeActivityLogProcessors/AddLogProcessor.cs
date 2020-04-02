using System;
using System.Linq;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.DictionaryManagement.Managers;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeActivityLogProcessors
{
    public class AddLogProcessor : WorkWithLogBaseProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue dateParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.Date);
            if (dateParameter.DateValue.Date < Constants.MinAllowedLogDate)
                return ExecuteCommandResult.CreateErrorObject($"Дата {dateParameter.DateValue:dd.MM.yyyy} " +
                    $"не может быть меньше {Constants.MinAllowedLogDate:dd.MM.yyyy}");
            if (dateParameter.DateValue.Date > DateTime.Now.Date)
                return ExecuteCommandResult.CreateErrorObject($"Дата {dateParameter.DateValue:dd.MM.yyyy} " +
                    $"не может быть больше сегодняшней даты");

            CommandParameterAndValue activityCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);
            CommandParameterAndValue valuesParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamValues);
            CommandParameterAndValue commentParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.Comment);

            // Get activity and it's parameters
            var actWithParams = LifeActivityManager.GetActivityAndParamsByUserCode(activityCodeParameter.IntValue, session.UserId);
            if (actWithParams.Activity == null)
                return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityCodeParameter.IntValue} не найдена");
            // Sort this parameters
            actWithParams.Parameters = actWithParams.Parameters.Where(n => n.Active).OrderBy(n => n.SortOrder).ToArray();
            if (actWithParams.Parameters.Length == 0)
                return ExecuteCommandResult.CreateErrorObject(
                    $"У деятельности \"{actWithParams.Activity.NameForUser}\" отсутствуют параметры.");

            // Read log values
            LifeActivityLogValue[] newLogValues = CreateEmptyLogValues(session.UserId, dateParameter.DateValue, actWithParams.Parameters);
            string fillLogValuesResult = FillLogValues(valuesParameter.Value, actWithParams.Parameters, newLogValues);
            if (!string.IsNullOrEmpty(fillLogValuesResult))
                return ExecuteCommandResult.CreateErrorObject(fillLogValuesResult);

            // Get activity log and it's parameters
            LifeActivityLogWithValues[] existLogs = LifeActivityLogManager.GetLogsForDate(
                actWithParams.Activity.Id, dateParameter.DateValue, session.UserId);

            string newComment = commentParameter != null ? commentParameter.Value : "";
            if (existLogs.Length > 0)
            {
                if (IsTheSameLogValues(newLogValues, newComment, existLogs) != null)
                {
                    return ExecuteCommandResult.CreateErrorObject(
                        $"На дату {dateParameter.DateValue:dd.MM.yyyy} " +
                        $"деятельность \"{actWithParams.Activity.NameForUser}\" с такими значениями уже зарегистрирована.");
                }
            }

            LifeActivityLog newLog = new LifeActivityLog();
            newLog.UserId = session.UserId;
            newLog.LifeActivityId = actWithParams.Activity.Id;
            newLog.Period = dateParameter.DateValue;
            newLog.Comment = newComment;
            LifeActivityLogManager.AddLog(newLog, newLogValues);

            Measure[] measures = MeasureManager.GetMeasuresForUser(session.UserId);
            string logTrace = ProcessorHelpers.PrintLog(
                newLog, newLogValues, actWithParams.Activity, actWithParams.Parameters, measures);
            Logger.Info("Added log: " + logTrace);

            return ExecuteCommandResult.CreateSuccessObject($"Добавлен лог {{ {logTrace} }}");
        }
    }
}

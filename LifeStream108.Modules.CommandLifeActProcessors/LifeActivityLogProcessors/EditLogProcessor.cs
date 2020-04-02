using System;
using System.Linq;
using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeActivityLogProcessors
{
    public class EditLogProcessor : WorkWithLogBaseProcessor
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

            CommandParameterAndValue activityLogCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityLogCode);
            CommandParameterAndValue valuesParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityParamValues);
            CommandParameterAndValue commentParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.Comment);

            var findLogResult = session.GetNumberDataValue(activityLogCodeParameter.Value);
            if (!string.IsNullOrEmpty(findLogResult.Error))
                return ExecuteCommandResult.CreateErrorObject(findLogResult.Error);

            LifeActivityLogWithValues logWithValues = LifeActivityLogManager.GetLogWithValues(findLogResult.Value, session.UserId);
            if (logWithValues.Log == null) throw new LifeStream108Exception(ErrorType.LifeActivityLogNotFound,
                $"Activity log with id {findLogResult.Value} for user {session.UserId} not found",
                $"Не удалось найти лог деятельности с кодом {activityLogCodeParameter.Value}", "Session data: " + session.Data);

            // Get activity and it's parameters
            var actWithParams = LifeActivityManager.GetActivityWithParams(logWithValues.Log.LifeActivityId, session.UserId);
            if (actWithParams.Activity == null)
            {
                Logger.Warn($"For user {session.UserId} activity with id {logWithValues.Log.LifeActivityId} not found");
                return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityLogCodeParameter.Value} не найдена");
            }

            // Sort this parameters
            actWithParams.Parameters = actWithParams.Parameters
                .Where(n => n.Active).OrderBy(n => n.SortOrder).ToArray();
            if (actWithParams.Parameters.Length == 0)
                return ExecuteCommandResult.CreateErrorObject(
                    $"У деятельности \"{actWithParams.Activity.NameForUser}\" отсутствуют параметры.");

            // Read log values
            LifeActivityLogValue[] newLogValues = CreateEmptyLogValues(session.UserId, dateParameter.DateValue, actWithParams.Parameters);
            string fillLogValuesResult = FillLogValues(valuesParameter.Value, actWithParams.Parameters, newLogValues);
            if (!string.IsNullOrEmpty(fillLogValuesResult))
                return ExecuteCommandResult.CreateErrorObject(fillLogValuesResult);

            string newComment = commentParameter != null ? commentParameter.Value : "";
            if (IsTheSameLogValues(newLogValues, newComment, new LifeActivityLogWithValues[] { logWithValues}) != null)
            {
                return ExecuteCommandResult.CreateErrorObject(
                    $"На дату {logWithValues.Log.Period:dd.MM.yyyy} " +
                    $"деятельность \"{actWithParams.Activity.NameForUser}\" с такими значениями уже зарегистрирована.");
            }
            // Copy new values
            foreach (LifeActivityLogValue editedLogValue in logWithValues.Values)
            {
                LifeActivityLogValue newLogValue = newLogValues.FirstOrDefault(n => n.ActivityParamId == editedLogValue.ActivityParamId);
                editedLogValue.NumericValue = newLogValue.NumericValue;
                editedLogValue.TextValue = newLogValue.TextValue;
            }
            // Save new values
            logWithValues.Log.Comment = newComment;
            LifeActivityLogManager.UpdateLog(logWithValues.Log, logWithValues.Values);

            return ExecuteCommandResult.CreateSuccessObject("Лог обновлён");
        }
    }
}

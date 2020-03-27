using System.Linq;
using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Modules.DictionaryManagement.Managers;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeActivityLogProcessors
{
    public class DeleteLogProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityLogCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityLogCode);

            var findLogResult = session.GetNumberDataValue(activityLogCodeParameter.Value);
            if (!string.IsNullOrEmpty(findLogResult.Error))
                return ExecuteCommandResult.CreateErrorObject(findLogResult.Error);

            var logWithValues = LifeActivityLogManager.GetLogWithValues(findLogResult.Value, session.UserId);
            if (logWithValues.Log == null) throw new LifeStream108Exception(ErrorType.LifeActivityLogNotFound,
                $"Activity log with id {findLogResult.Value} for user {session.UserId} not found",
                $"Не удалось найти лог деятельности с кодом {activityLogCodeParameter.Value}", "Session data: " + session.Data);

            var actWithParams = LifeActivityManager.GetActivityWithParams(logWithValues.Log.LifeActivityId, session.UserId);
            Measure[] measures = MeasureManager.GetMeasuresForUser(session.UserId);
            string logTrace = ProcessorHelpers.PrintLog(
                logWithValues.Log, logWithValues.Values, actWithParams.Activity, actWithParams.Parameters, measures);
            if (logWithValues.Log.Active)
            {
                logWithValues.Log.Active = false;
                LifeActivityLogManager.UpdateLog(logWithValues.Log);
                Logger.Info("Added log: " + logTrace);

                return ExecuteCommandResult.CreateSuccessObject($"Лог {{ {logTrace} }} деактивирован");
            }
            else return ExecuteCommandResult.CreateErrorObject($"Лог {{ {logTrace} }} уже был деактивирован");
        }
    }
}

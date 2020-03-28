using System.Collections.Generic;
using System.Linq;
using System.Text;
using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.DictionaryManagement.Managers;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.ReportProcessors
{
    public class ReportForActivitiesProcessor : ReporBaseProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue periodFromParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.Period);

            CommandParameterAndValue actCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);

            int onlyActivityId = 0;
            if (actCodeParameter != null)
            {
                LifeActivity activity = LifeActivityManager.GetActivityByUserCode(actCodeParameter.IntValue, session.UserId);
                if (activity == null)
                    return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {actCodeParameter.IntValue} не найдена");

                onlyActivityId = activity.Id;
            }

            LifeGroup[] allGroups = LifeGroupManager.GetGroupsForUser(session.UserId);
            LifeActivity[] allActivities = LifeActivityManager.GetActivitiesForUser(session.UserId);
            LifeActivityParameter[] allActivityParams = LifeActivityParameterManager.GetParametersForUser(session.UserId);
            Measure[] allMeasures = MeasureManager.GetMeasuresForUser(session.UserId);

            DatePeriod period = periodFromParameter.PeridValue;
            LifeActivityLog[] logs = LifeActivityLogManager.GetLogsForPeriod(
                period.From, period.To, session.UserId, onlyActivityId).Where(n => n.Active).ToArray();
            var groupedLogs = from log in logs
                              group log by log.LifeActivityId into grp
                              select new { ActivityId = grp.Key, Logs = grp.ToArray() };

            LifeActivityLogValue[] allLogValues =
                LifeActivityLogManager.GetLogValuesForPeriod(period.From, period.To, session.UserId);

            ExecuteCommandResult successCommandResult = new ExecuteCommandResult { Success = true };

            List<LifeActivityLogValue> allValues = new List<LifeActivityLogValue>();
            StringBuilder sbReport = new StringBuilder($"<b>Выписка за {period.ToString()}:</b>\r\n");
            foreach (var groupedItem in groupedLogs)
            {
                LifeActivityLog[] currentLogs = groupedItem.Logs.OrderBy(n => n.RegTime).ToArray();
                LifeActivity currentActivity = allActivities.FirstOrDefault(n => n.Id == groupedItem.ActivityId);
                LifeActivityParameter[] currentActivityParams =
                    allActivityParams.Where(n => n.ActivityId == currentActivity.Id).OrderBy(n => n.SortOrder).ToArray();
                allValues.Clear();
                foreach (LifeActivityLog log in currentLogs)
                {
                    int logCode = successCommandResult.AddSessionValue(log.Id);

                    LifeActivityLogValue[] logValues = allLogValues.Where(n => n.ActivityLogId == log.Id).ToArray();

                    sbReport.Append($"[{logCode}] {log.Period.ToString("dd.MM.yyyy")}, {currentActivity.NameForUser}: " +
                        $"{ProcessorHelpers.PrintLogValues(logValues, currentActivityParams, allMeasures)}" +
                        $"{(!string.IsNullOrEmpty(log.Comment) ? ", " + log.Comment : "")}\r\n");

                    if (currentLogs.Length > 1)
                    {
                        allValues.AddRange(logValues);
                    }
                }

                if (currentLogs.Length > 1)
                {
                    string totalsResult = CalcTotalsForActivityLogs(allValues, currentActivityParams, allMeasures);
                    if (!string.IsNullOrEmpty(totalsResult)) sbReport.Append($"    <b>Итого</b>: {totalsResult}\r\n");
                }
            }

            successCommandResult.ResponseMessage = sbReport.ToString();
            return successCommandResult;
        }
    }
}

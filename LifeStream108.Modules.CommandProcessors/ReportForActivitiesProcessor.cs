using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities;
using LifeStream108.Modules.DictionaryManagement.Managers;
using LifeStream108.Modules.LifeActivityManagement.Managers;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class ReportForActivitiesProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue periodFromParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.Period);

            CommandParameterAndValue activityCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);

            int onlyActivityId = 0;
            if (activityCodeParameter != null)
            {
                LifeActivity activity = LifeActivityManager.GetActivityByUserCode(activityCodeParameter.IntValue, session.UserId);
                if (activity == null)
                    return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityCodeParameter.IntValue} не найдена");

                onlyActivityId = activity.Id;
            }

            LifeGroup[] allGroups = LifeGroupManager.GetGroupsForUser(session.UserId);
            LifeActivity[] allActivities = LifeActivityManager.GetActivitiesForUser(session.UserId);
            LifeActivityParameter[] allActivityParams = LifeActivityParameterManager.GetParametersForUser(session.UserId);
            Measure[] allMeasures = MeasureManager.GetMeasuresForUser(session.UserId);

            DatePeriod period = periodFromParameter.PeridValue;
            LifeActivityLog[] logs = LifeActivityLogManager.GetLogsForPeriod(period.From, period.To, session.UserId, onlyActivityId)
                .Where(n => n.Active).ToArray();
            var groupedLogs = from log in logs
                              group log by log.LifeActivityId into grp
                              select new { ActivityId = grp.Key, Logs = grp.ToArray() };

            LifeActivityLogValue[] allLogValues = LifeActivityLogManager.GetLogValuesForPeriod(period.From, period.To, session.UserId);

            ExecuteCommandResult successCommandResult = new ExecuteCommandResult { Success = true };

            List<LifeActivityLogValue> allValues = new List<LifeActivityLogValue>();
            StringBuilder sbReport = new StringBuilder($"<b>Выписка за {period.From.ToString("dd.MM.yyyy")}" +
                $"{(period.To != period.From ? " - " + period.To.ToString("dd.MM.yyyy") : "")}:</b>\r\n");
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
                    string totalsResult = CalcTotals(allValues, currentActivityParams, allMeasures);
                    if (!string.IsNullOrEmpty(totalsResult)) sbReport.Append($"    <b>Итого</b>: {totalsResult}\r\n");
                }
            }

            successCommandResult.ResponseMessage = sbReport.ToString();
            return successCommandResult;
        }

        private static string CalcTotals(List<LifeActivityLogValue> values, LifeActivityParameter[] activityParams,
            Measure[] measures)
        {
            if (activityParams == null)
            {
                Logger.Warn("Impossible calc totals couse params is null");
                return null;
            }

            StringBuilder sbResult = new StringBuilder();
            Dictionary<int, (double Value, string Measure)> dicTotals = new Dictionary<int, (double Value, string Measure)>();
            LifeActivityParameter[] paramList = activityParams.Where(n => n.Fuction.ToUpper() == "SUM").OrderBy(n => n.SortOrder).ToArray();
            for (int i = 0; i < paramList.Length; i++)
            {
                LifeActivityParameter param = paramList[i];
                double sum = values.Where(n => n.ActivityParamId == param.Id).Sum(n => n.NumericValue);
                Measure measure = measures.FirstOrDefault(n => n.Id == param.MeasureId);

                sbResult.Append($"{DataFormatter.FormatNumber(sum)} {ProcessorHelpers.PrintMeasure(sum, measure)}");
                if (i < paramList.Length - 1) sbResult.Append(", ");
            }
            return sbResult.ToString();
        }
    }
}

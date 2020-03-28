using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.DictionaryManagement.Managers;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using System.Linq;

namespace LifeStream108.Modules.CommandLifeActProcessors.ReportProcessors
{
    public class SummaryReportForActProcessor : ReporBaseProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue actCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);

            CommandParameterAndValue periodFromParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.Period);

            var act = LifeActivityManager.GetActivityAndParamsByUserCode(actCodeParameter.IntValue, session.UserId);

            DatePeriod period = periodFromParameter.PeridValue;
            LifeActivityLog[] logs = LifeActivityLogManager.GetLogsForPeriod(
                period.From, period.To, session.UserId, act.Activity.Id).Where(n => n.Active).ToArray();
            LifeActivityLogValue[] allLogValues =
                LifeActivityLogManager.GetLogValuesForPeriod(period.From, period.To, session.UserId);

            Measure[] allMeasures = MeasureManager.GetMeasuresForUser(session.UserId);

            string startString = $"Итог по '<b>{act.Activity.Name}</b>'\r\nза <b>{period.ToString()}:</b>\r\n";
            string nextString = CalcTotalsForActivityLogs(allLogValues, act.Parameters, allMeasures);
            return ExecuteCommandResult.CreateSuccessObject(startString + nextString);
        }
    }
}

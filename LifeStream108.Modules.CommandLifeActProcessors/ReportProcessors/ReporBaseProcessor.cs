using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.CommandProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeStream108.Modules.CommandLifeActProcessors.ReportProcessors
{
    public abstract class ReporBaseProcessor : BaseCommandProcessor
    {
        protected static string CalcTotalsForActivityLogs(IEnumerable<LifeActivityLogValue> values, LifeActivityParameter[] activityParams,
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

        protected static int CountActiveDays(LifeActivityLog[] logs)
        {
            DateTime[] activeDates = logs.Select(n => n.Period.Date).ToArray();
            if (activeDates.Length == 0) return 0;

            return activeDates.Distinct().Count();
        }
    }
}

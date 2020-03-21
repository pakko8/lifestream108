using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using System;
using System.Globalization;

namespace LifeStream108.Modules.CommandProcessors
{
    internal abstract class WorkWithActivityLogBaseProcessor : BaseCommandProcessor
    {
        protected static LifeActivityLogWithValues IsTheSameLogValues(LifeActivityLogValue[] newLogValues, string newComment,
            LifeActivityLogWithValues[] existLogs)
        {
            foreach (LifeActivityLogWithValues existLog in existLogs)
            {
                if (existLog.IsEquals(newLogValues, newComment)) return existLog;
            }
            return null;
        }

        protected static string FillLogValues(string valuesString, LifeActivityParameter[] activityParameters, LifeActivityLogValue[] logValues)
        {
            string[] valueParts = valuesString.Split(new[] { '+' });
            if (valueParts.Length != activityParameters.Length)
                return $"Количество значений для деятельности должно быть {activityParameters.Length}: " +
                    $"{ProcessorHelpers.FormatLifyActivityNames(activityParameters)}";

            for (int i = 0; i < activityParameters.Length; i++)
            {
                string logValueString = valueParts[i].Trim();
                if (string.IsNullOrEmpty(logValueString)) return $"Значение для параметра \"{activityParameters[i].NameForUser}\" не может быть пустым";

                if (activityParameters[i].DataType == DataType.Double)
                {
                    logValueString = logValueString.Replace(',', '.');
                    if (!double.TryParse(logValueString, System.Globalization.NumberStyles.Any, new CultureInfo("en-US"), out double logValueNumeric))
                        return $"Значение для параметра \"{activityParameters[i].NameForUser}\" должно быть числом";

                    logValues[i].NumericValue = logValueNumeric;
                }
                else
                {
                    logValues[i].TextValue = logValueString;
                }
            }
            return null;
        }

        protected static LifeActivityLogValue[] CreateEmptyLogValues(int userId, DateTime period, LifeActivityParameter[] activityParameters)
        {
            LifeActivityLogValue[] resultLogValues = new LifeActivityLogValue[activityParameters.Length];
            for (int i = 0; i < activityParameters.Length; i++)
            {
                resultLogValues[i] = new LifeActivityLogValue
                {
                    UserId = userId,
                    ActivityParamId = activityParameters[i].Id,
                    Period = period
                };
            }
            return resultLogValues;
        }
    }
}

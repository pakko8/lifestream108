using NLog;
using System.Linq;

namespace LifeStream108.Libs.Entities
{
    public class LifeActivityLogWithValues
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public LifeActivityLog Log { get; set; }

        public LifeActivityLogValue[] Values { get; set; }

        public bool IsEquals(LifeActivityLogValue[] logValues, string comment)
        {
            if (comment != Log.Comment) return false;
            foreach (LifeActivityLogValue logValue in logValues)
            {
                LifeActivityLogValue thisLogValue = Values.FirstOrDefault(n => n.ActivityParamId == logValue.ActivityParamId);
                if (thisLogValue == null)
                {
                    Logger.Warn($"For user {Log.UserId} for log {Log.Id} not found value for parameter {logValue.ActivityParamId}");
                    return false;
                }
                if (logValue.NumericValue != thisLogValue.NumericValue || logValue.TextValue != thisLogValue.TextValue) return false;
            }
            return true;
        }
    }
}

using NLog;

namespace LifeStream108.Libs.Entities
{
    internal static class LoggerAccess
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    }
}

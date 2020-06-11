using NLog;

namespace LifeStream108.Services.NewsCheckerWorker
{
    internal static class InnerHelpers
    {
        public static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    }
}

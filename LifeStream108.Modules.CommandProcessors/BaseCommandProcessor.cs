using LifeStream108.Libs.Entities.SessionEntities;
using NLog;

namespace LifeStream108.Modules.CommandProcessors
{
    public abstract class BaseCommandProcessor
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public abstract ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session);
    }
}

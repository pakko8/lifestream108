using LifeStream108.Libs.Entities.SessionEntities;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class HelpProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            return ExecuteCommandResult.CreateErrorObject("Для получения списка команд введите <b>cmd</b>");
        }
    }
}

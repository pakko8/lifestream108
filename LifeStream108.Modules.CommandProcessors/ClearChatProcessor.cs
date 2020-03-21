using LifeStream108.Libs.Entities.SessionEntities;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class ClearChatProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            return new ExecuteCommandResult
            {
                Success = true,
                SpecialCommandForTelegramBot = SpecialCommandForTelegramBot.ClearChat
            };
        }
    }
}

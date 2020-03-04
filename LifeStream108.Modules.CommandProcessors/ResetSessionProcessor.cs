using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class ResetSessionProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            session.LastLifeActivityId = 0;
            session.LastLifeGroupId = 0;

            return ExecuteCommandResult.CreateSuccessObject("Сессия очищена");
        }
    }
}

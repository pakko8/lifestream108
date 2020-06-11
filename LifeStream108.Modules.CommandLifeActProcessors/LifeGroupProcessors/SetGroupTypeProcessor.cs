using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Modules.CommandProcessors;
using System;
using System.Linq;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeGroupProcessors
{
    public class SetGroupTypeProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue groupCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupCode);

            throw new NotImplementedException();
        }
    }
}

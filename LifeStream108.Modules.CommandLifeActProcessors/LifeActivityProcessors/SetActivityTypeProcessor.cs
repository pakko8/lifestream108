using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Modules.CommandProcessors;
using System;
using System.Linq;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeActivityProcessors
{
    public class SetActivityTypeProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);

            throw new NotImplementedException();
        }
    }
}

using System.Linq;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeGroupProcessors
{
    public class RenameGroupProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue groupCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupCode);
            CommandParameterAndValue groupNameParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupName);

            LifeGroup foundGroup = LifeGroupManager.GetGroupByCode(groupCodeParameter.IntValue, session.UserId);
            if (foundGroup == null)
                return ExecuteCommandResult.CreateErrorObject($"Группа с кодом {groupCodeParameter.IntValue} не найдена");

            string oldName = foundGroup.NameForUser;
            foundGroup.Name = groupNameParameter.Value;
            LifeGroupManager.UpdateGroup(foundGroup);

            return ExecuteCommandResult.CreateSuccessObject($"Название группы \"{oldName}\" изменено \"{foundGroup.NameForUser}\"");
        }
    }
}

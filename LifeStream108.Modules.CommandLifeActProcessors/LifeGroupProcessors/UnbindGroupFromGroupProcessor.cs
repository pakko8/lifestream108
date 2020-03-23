using System.Linq;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeGroupProcessors
{
    public class UnbindGroupFromGroupProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue whatGroupCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupCode);

            CommandParameterAndValue fromGroupCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupCode2);

            LifeGroup whatGroup = LifeGroupManager.GetGroupByCode(whatGroupCodeParameter.IntValue, session.UserId);
            if (whatGroup == null)
                return ExecuteCommandResult.CreateErrorObject($"Группа с кодом {whatGroupCodeParameter.IntValue} не найдена");
            LifeGroup fromGroup = LifeGroupManager.GetGroupByCode(fromGroupCodeParameter.IntValue, session.UserId);
            if (fromGroup == null)
                return ExecuteCommandResult.CreateErrorObject($"Группа с кодом {fromGroupCodeParameter.IntValue} не найдена");

            LifeGroupAtGroup groupAtGroup =
                LifeGroupAtGroupManager.GetGroupAtGroupByGroups(whatGroup.Id, fromGroup.Id, session.UserId);
            if (groupAtGroup == null)
                return ExecuteCommandResult.CreateErrorObject(
                    $"Связь между группами \"{whatGroup.NameForUser}\" и \"{fromGroup.NameForUser}\" отсутствует");

            LifeGroup group = LifeGroupManager.GetGroup(groupAtGroup.LifeGroupId, session.UserId);
            LifeGroup parentGroup = LifeGroupManager.GetGroup(groupAtGroup.ParentLifeGroupId, session.UserId);

            if (groupAtGroup.ParentLifeGroupId == 0)
                return ExecuteCommandResult.CreateSuccessObject($"Группа \"{group.NameForUser}\" уже отвязана от группы \"{parentGroup.NameForUser}\"");

            groupAtGroup.ParentLifeGroupId = 0;
            LifeGroupAtGroupManager.UpdateGroupAtGroup(groupAtGroup);

            return ExecuteCommandResult.CreateSuccessObject($"Группа \"{group.NameForUser}\" отвязана от группы \"{parentGroup.NameForUser}\"");
        }
    }
}

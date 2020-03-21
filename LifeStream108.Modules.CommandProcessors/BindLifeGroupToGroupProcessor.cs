using System.Linq;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class BindLifeGroupToGroupProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue whatGroupCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupCode);
            CommandParameterAndValue toGroupCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupCode2);

            LifeGroup whatGroup = LifeGroupManager.GetGroupByCode(whatGroupCodeParameter.IntValue, session.UserId);
            if (whatGroup == null)
                return ExecuteCommandResult.CreateErrorObject($"Группа с кодом {whatGroupCodeParameter.IntValue} не найдена");
            LifeGroup toGroup = LifeGroupManager.GetGroupByCode(toGroupCodeParameter.IntValue, session.UserId);
            if (toGroup == null)
                return ExecuteCommandResult.CreateErrorObject($"Группа с кодом {toGroupCodeParameter.IntValue} не найдена");

            LifeGroupAtGroup groupAtGroup = LifeGroupAtGroupManager.GetGroupAtGroupByGroups(
                whatGroup.Id, toGroup.Id, session.UserId);
            if (groupAtGroup != null)
                return ExecuteCommandResult.CreateErrorObject($"Группа \"{whatGroup.NameForUser}\" уже привязана к группе \"{toGroup.NameForUser}\"");

            groupAtGroup = LifeGroupAtGroupManager.GetGroupAtGroupByGroups(whatGroup.Id, 0, session.UserId);
            if (groupAtGroup != null) // Group without parent group
            {
                groupAtGroup.ParentLifeGroupId = toGroup.Id;
                LifeGroupAtGroupManager.UpdateGroupAtGroup(groupAtGroup);
            }
            else // New group to group
            {
                groupAtGroup = new LifeGroupAtGroup
                {
                    UserId = session.UserId,
                    LifeGroupId = whatGroup.Id,
                    ParentLifeGroupId = toGroup.Id
                };
                LifeGroupAtGroupManager.AddGroupAtGroup(groupAtGroup);
            }

            return ExecuteCommandResult.CreateSuccessObject($"Группа \"{whatGroup.NameForUser}\" привязана к группе \"{toGroup.NameForUser}\"");
        }
    }
}

using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using System.Linq;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.LifeActivityManagement;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeGroupProcessors
{
    public class AddGroupProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue groupNameParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupName);
            LifeGroup newGroup = LifeGroupManager.GetGroupByName(groupNameParameter.Value, session.UserId);
            if (newGroup == null)
            {
                newGroup = new LifeGroup
                {
                    UserId = session.UserId,
                    Name = groupNameParameter.Value
                };
                LifeGroupManager.AddGroup(newGroup);

                return ExecuteCommandResult.CreateSuccessObject($"Группа \"{newGroup.NameForUser}\" добавлена.");
            }
            else if (!newGroup.Active)
            {
                newGroup.Active = true;
                LifeGroupManager.UpdateGroup(newGroup);
                return ExecuteCommandResult.CreateSuccessObject(
                    $"Группа \"{newGroup.NameForUser}\" восстановлена после удаления.");
            }
            else return ExecuteCommandResult.CreateErrorObject($"Группа \"{newGroup.NameForUser}\" уже существует.");
        }
    }
}

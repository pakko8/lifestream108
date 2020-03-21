using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using System.Linq;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class AddLifeGroupProcessor : BaseCommandProcessor
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

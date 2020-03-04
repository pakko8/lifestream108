using LifeStream108.Libs.Entities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using System.Linq;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class DeleteLifeGroupProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue groupCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupCode);

            LifeGroup groupToDelete = LifeGroupManager.GetGroupByCode(groupCodeParameter.IntValue, session.UserId);
            if (groupToDelete == null)
                return ExecuteCommandResult.CreateErrorObject($"Группа с кодом {groupCodeParameter.IntValue} не найдена");

            groupToDelete.Active = false;
            LifeGroupManager.UpdateGroup(groupToDelete);

            return ExecuteCommandResult.CreateSuccessObject("Группа деактивирована");
        }
    }
}

﻿using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using System.Linq;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.LifeActivityManagement;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeGroupProcessors
{
    internal class DeleteGroupProcessor : BaseCommandProcessor
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

using System.Linq;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors
{
    public class BindLifeActivityToGroupProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityCode);
            CommandParameterAndValue groupCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupCode);

            LifeActivity activity = LifeActivityManager.GetActivityByUserCode(activityCodeParameter.IntValue, session.UserId);
            if (activity == null)
                return ExecuteCommandResult.CreateErrorObject($"Деятельность с кодом {activityCodeParameter.IntValue} не найдена");
            LifeGroup group = LifeGroupManager.GetGroupByCode(groupCodeParameter.IntValue, session.UserId);
            if (group == null)
                return ExecuteCommandResult.CreateErrorObject($"Группа с кодом {groupCodeParameter.IntValue} не найдена");

            LifeGroupAtGroup[] parentGroups = LifeGroupAtGroupManager.GetGroupAtGroupsByGroup(group.Id, session.UserId);
            if (parentGroups.Length == 1)
            {
                if (activity.LifeGroupAtGroupId == parentGroups[0].Id)
                    return ExecuteCommandResult.CreateErrorObject($"Деятельность \"{activity.NameForUser}\" уже привязана к группе \"{group.NameForUser}\"");

                activity.LifeGroupAtGroupId = parentGroups[0].Id;
                LifeActivityManager.UpdateActivity(activity);

                return ExecuteCommandResult.CreateSuccessObject($"Деятельность \"{activity.NameForUser}\" привязана к группе \"{group.NameForUser}\"");
            }
            else
            {
                ChoiceItem[] choiceItems = new ChoiceItem[parentGroups.Length];
                for (int i = 0; i < parentGroups.Length; i++)
                {
                    choiceItems[i] = new ChoiceItem();
                    choiceItems[i].Text =
                        $"К группе \"{ProcessorHelpers.GetFullLifeGroupName(group.Id, parentGroups[i].LifeGroupId, session.UserId)}\"";
                    choiceItems[i].Command =
                        $"BackMethod=BindActivityToGroup;Params={activity.Id};{parentGroups[i].Id};{session.UserId}";
                }
                ExecuteCommandResult executeResult = ExecuteCommandResult.CreateSuccessObject(
                    $"Деятельность \"{activity.NameForUser}\" можно привязать к нескольким группам. К какой из них привязать?");
                executeResult.ChoiceItemList = choiceItems;
                return executeResult;
            }
        }
    }
}

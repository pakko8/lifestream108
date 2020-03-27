using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.LifeActivityManagement.Managers;

namespace LifeStream108.Modules.CommandLifeActProcessors
{
    public class BackgroudCommandsProcessor
    {
        public ExecuteCommandResult BindActivityToGroup(int activityId, int groupAtGroupId, int userId)
        {
            string groupName = ProcessorHelpers.GetFullLifeGroupName(groupAtGroupId, userId);

            LifeActivity activity = LifeActivityManager.GetActivityByUserCode(activityId, userId);
            activity.LifeGroupAtGroupId = groupAtGroupId;
            LifeActivityManager.UpdateActivity(activity);

            return ExecuteCommandResult.CreateSuccessObject($"Деятельность \"{activity.NameForUser}\" добавлена к группе \"{groupName}\"");
        }
    }
}

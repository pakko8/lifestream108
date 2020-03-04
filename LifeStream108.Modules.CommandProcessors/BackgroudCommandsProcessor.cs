﻿using LifeStream108.Libs.Entities;
using LifeStream108.Modules.LifeActivityManagement.Managers;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class BackgroudCommandsProcessor
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

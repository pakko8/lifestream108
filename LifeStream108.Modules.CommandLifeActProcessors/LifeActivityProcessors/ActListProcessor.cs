using System.Collections.Generic;
using System.Linq;
using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeActivityProcessors
{
    public class ActListProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            LifeActivity[] activities = LifeActivityManager.GetActivitiesForUser(session.UserId);
            activities = activities.Where(n => n.Active).ToArray();

            List<string> activityNames = new List<string>();
            foreach (LifeActivity activity in activities)
            {
                activityNames.Add($"[{activity.UserCode}] <b>{activity.Name}</b>");
            }

            return ExecuteCommandResult.CreateSuccessObject("Список деятельностей:\r\n" + CollectionUtils.Array2String(activityNames, "\r\n"));
        }
    }
}

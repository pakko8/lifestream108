using System.Linq;
using System.Text;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors
{
    public class FindLifeActivityProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue activityNameParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeActivityName);

            LifeActivity[] foundActivities = LifeActivityManager.FindActivitiesByName(activityNameParameter.Value, session.UserId);
            if (foundActivities.Length == 0)
                return ExecuteCommandResult.CreateErrorObject($"Не найдено деятельностей, которые содержат фразу \"{activityNameParameter.Value}\"");

            StringBuilder sbResult = new StringBuilder("Подходящие деятельности:\r\n:");
            foreach (LifeActivity activity in foundActivities)
            {
                sbResult.Append($"[{activity.UserCode}] {FormatActivityName(activity.NameForUser, activityNameParameter.Value)}\r\n");
            }

            return ExecuteCommandResult.CreateSuccessObject(sbResult.ToString());
        }

        private string FormatActivityName(string activityName, string containsString)
        {
            int index = activityName.ToUpper().IndexOf(containsString.ToUpper());
            string namePart = activityName.Substring(index, containsString.Length);
            return activityName.Replace(namePart, $"<b>{namePart}</b>");
        }
    }
}

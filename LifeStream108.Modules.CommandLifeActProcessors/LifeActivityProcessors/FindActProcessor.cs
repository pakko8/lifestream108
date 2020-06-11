using System.Linq;
using System.Text;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeActivityProcessors
{
    public class FindActProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue searchPhraseParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.SearchPhrase);

            LifeActivity[] foundActivities =
                LifeActivityManager.FindActivitiesByName(searchPhraseParameter.Value, session.UserId);
            if (foundActivities.Length == 0) return ExecuteCommandResult.CreateErrorObject(
                $"Не найдено деятельностей, содержащих фразу \"{searchPhraseParameter.Value}\"");

            StringBuilder sbResult = new StringBuilder("Подходящие деятельности:\r\n:");
            foreach (LifeActivity activity in foundActivities)
            {
                sbResult.Append($@"[{activity.UserCode}] {FormatActivityName(activity.NameForUser, searchPhraseParameter.Value)}
                    {ProcessorHelpers.PrintPeriodicityType(activity.PeriodType, ", ")}\r\n");
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

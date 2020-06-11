using System.Linq;
using System.Text;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using LifeStream108.Modules.CommandProcessors;

namespace LifeStream108.Modules.CommandLifeActProcessors.LifeGroupProcessors
{
    public class GroupListProcessor : BaseCommandProcessor
    {
        private LifeGroup[] _userGroups;
        private LifeGroupAtGroup[] _groupAtGroups;
        private LifeActivity[] _userActivities;

        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            _groupAtGroups = LifeGroupAtGroupManager.GetGroupsAtGroupsForUser(session.UserId);
            LifeGroupAtGroup[] topLevelGroupAtGroupList = _groupAtGroups.Where(n => n.ParentLifeGroupId == 0).ToArray();
            if (topLevelGroupAtGroupList.Length == 0)
                return ExecuteCommandResult.CreateErrorObject("Отсутствуют группы верхнего уровня");

            _userGroups = LifeGroupManager.GetGroupsForUser(session.UserId);

            bool showActivities = true;
            if (showActivities)
            {
                _userActivities = LifeActivityManager.GetActivitiesForUser(session.UserId);
            }

            StringBuilder sbGroupsAsText = new StringBuilder();
            PrintGroups(topLevelGroupAtGroupList, 1, true, sbGroupsAsText);

            return ExecuteCommandResult.CreateSuccessObject("Список групп:\r\n" + sbGroupsAsText.ToString());

            /*
            List<ChoiceItem> choiceItems = new List<ChoiceItem>();
            foreach (LifeGroup userGroup in userGroups)
            {
                choiceItems.Add(new ChoiceItem
                {
                    Command = $"CHOOSE={userGroup.Id}",
                    Text = $"[{userGroup.Id}] {userGroup.Name}"
                });
            }

            executeResult.ChoiceItemList = choiceItems.ToArray();
            executeResult.ChoiceListShowColumnsCount = 1;
            */
        }

        private void PrintGroups(LifeGroupAtGroup[] groupAtGroupList, int level, bool showActivities, StringBuilder sbGroupsAsText)
        {
            foreach (LifeGroupAtGroup groupAtGroup in groupAtGroupList)
            {
                LifeGroup currentGroup = _userGroups.FirstOrDefault(n => n.Id == groupAtGroup.LifeGroupId);
                string groupOffset = new string(' ', 6 * (level - 1));
                sbGroupsAsText.Append(
                    $@"{groupOffset}{(groupOffset.Length > 0 ? "* " : "")}[{currentGroup.UserCode}] <b>{currentGroup.Name}</b>
                        {ProcessorHelpers.PrintFinancialType(currentGroup.FinanceType, ", ")}\r\n");

                if (showActivities)
                {
                    LifeActivity[] thisGroupActivities =
                        _userActivities.Where(n => n.LifeGroupAtGroupId == groupAtGroup.Id && n.Active).ToArray();
                    foreach (LifeActivity activity in thisGroupActivities)
                    {
                        string activityOffset = new string(' ', 6 * (level));
                        sbGroupsAsText.Append($"{activityOffset}- [{activity.UserCode}] <i>{activity.Name}</i>\r\n");
                    }
                }

                LifeGroupAtGroup[] nextLevelGroupAtGroups = _groupAtGroups.Where(n => n.ParentLifeGroupId == groupAtGroup.LifeGroupId).ToArray();
                if (nextLevelGroupAtGroups.Length > 0)
                {
                    PrintGroups(nextLevelGroupAtGroups, level + 1, showActivities, sbGroupsAsText);
                }
            }
        }
    }
}

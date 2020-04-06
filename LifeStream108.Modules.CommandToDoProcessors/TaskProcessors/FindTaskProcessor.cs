using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.ToDoListManagement.Managers;
using System.Linq;
using System.Text;

namespace LifeStream108.Modules.CommandToDoProcessors.TaskProcessors
{
    public class FindTaskProcessor : BaseCommandProcessor
    {
        private const int MaxFoundTasksCount = 30;

        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue searchPhraseParameter = commandParameters.FirstOrDefault(
               n => n.Parameter.ParameterCode == CommandParameterCode.SearchPhrase);

            ToDoTask[] foundTasks = ToDoTaskManager.FindTasks(
                searchPhraseParameter.Value, MaxFoundTasksCount + 10, session.UserId);
            if (foundTasks.Length == 0) return ExecuteCommandResult.CreateErrorObject(
                $"Не найдено задач, содержащих фразу \"{searchPhraseParameter.Value}\"");
            if (foundTasks.Length > MaxFoundTasksCount) return ExecuteCommandResult.CreateErrorObject(
                 $"Найдено более {MaxFoundTasksCount}задач. Уточните поиск, чтобы их было не более \"{MaxFoundTasksCount}\"");

            ToDoCategory[] categories = ToDoCategoryManager.GetUserCategories(session.UserId);
            ToDoList[] lists = ToDoListManager.GetUserLists(session.UserId);

            ExecuteCommandResult successCommandResult = new ExecuteCommandResult { Success = true };

            FoundItem[] foundItems = new FoundItem[foundTasks.Length];
            for (int i = 0; i < foundTasks.Length; i++)
            {
                ToDoList list = lists.FirstOrDefault(n => n.Id == foundTasks[i].ListId);
                ToDoCategory category = categories.FirstOrDefault(n => n.Id == list.CategoryId);

                int taskCode = successCommandResult.AddSessionValue(foundTasks[i].Id);

                FoundItem foundItem = new FoundItem();
                foundItem.CategoryCode = category.UserCode;
                foundItem.CategoryName = category.Name;
                foundItem.ListCode = list.UserCode;
                foundItem.ListName = list.Name;
                foundItem.TaskCode = taskCode;
                foundItem.TaskTitle = foundTasks[i].Title;
                foundItems[i] = foundItem;
            }

            var groupedItemsByCat = from task in foundItems
                                    group task by task.CategoryCode into grp
                                    select new { CategoryCode = grp.Key, Tasks = grp.ToArray() };
            StringBuilder sbSearchResult = new StringBuilder();
            foreach (var groupedItemByCat in groupedItemsByCat)
            {
                string categoryName = groupedItemByCat.Tasks[0].CategoryName;
                sbSearchResult.Append($"\r\n--- [{groupedItemByCat.CategoryCode}] <b>{categoryName}</b> ----------\r\n");

                var groupedItemsByList = from task in groupedItemByCat.Tasks
                                         group task by task.ListCode into grp
                                         select new { ListCode = grp.Key, Tasks = grp.ToArray() };
                foreach (var groupedItemByList in groupedItemsByList)
                {
                    string listName = groupedItemByList.Tasks[0].ListName;
                    sbSearchResult.Append($"    -[{groupedItemByList.ListCode}] <b>{listName}</b>\r\n");

                    foreach (FoundItem item in groupedItemByList.Tasks)
                    {
                        sbSearchResult.Append(
                            $"        *[{item.TaskCode}] <b>{TelegramUtils.RemoveUnsafeSigns(item.TaskTitle)}</b>\r\n");
                    }
                }
            }

            successCommandResult.ResponseMessage = sbSearchResult.ToString();
            return successCommandResult;
        }

        private class FoundItem
        {
            public int CategoryCode { get; set; }

            public string CategoryName { get; set; }

            public int ListCode { get; set; }

            public string ListName { get; set; }

            public int TaskCode { get; set; }

            public string TaskTitle { get; set; }
        }
    }
}

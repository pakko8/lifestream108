using System.Linq;
using System.Text;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.ToDoListManagement;

namespace LifeStream108.Modules.CommandToDoProcessors
{
    public class ListsProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue categoryCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.ToDoCategoryCode);

            ToDoCategory category = ToDoCategoryManager.GetCategoryByCode(categoryCodeParameter.IntValue, session.UserId);
            if (category == null) return ExecuteCommandResult.CreateErrorObject(
                $"Категория с кодом {categoryCodeParameter.IntValue} не найдена");

            ToDoList[] lists = ToDoListManager.GetCategoryLists(category.Id);
            StringBuilder sbLists = new StringBuilder();
            foreach (ToDoList list in lists.OrderBy(n => n.SortOrder))
            {
                sbLists.Append($"[{list.UserCode}] <b>{list.Name}</b>\r\n");
            }
            return ExecuteCommandResult.CreateSuccessObject("Списки:\r\n" + sbLists.ToString());
        }
    }
}

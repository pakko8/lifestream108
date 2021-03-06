﻿using System.Linq;
using System.Text;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.ToDoListManagement;

namespace LifeStream108.Modules.CommandToDoProcessors
{
    public class CategoryListProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            ToDoCategory[] categories = ToDoCategoryManager.GetUserCategories(session.UserId);
            StringBuilder sbCategogories = new StringBuilder();
            foreach (ToDoCategory category in categories.OrderBy(n => n.SortOrder))
            {
                sbCategogories.Append($"[{category.UserCode}] <b>{category.Name}</b>\r\n");
            }
            return ExecuteCommandResult.CreateSuccessObject("Список категорий:\r\n" + sbCategogories.ToString());
        }
    }
}

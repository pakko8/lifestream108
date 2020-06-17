using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Data;
using System.Linq;

namespace LifeStream108.Modules.ToDoListManagement
{
    public static class ToDoListManager
    {
        private const string TableName = "todo_list.todo_lists";

        public static ToDoList GetList(int listId)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where id={listId}", ReadList);
        }

        public static ToDoList[] GetUserLists(int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from list in session.Query<ToDoList>()
                            where list.UserId == userId
                            select list;
                return query.ToArray();
            }
        }

        public static ToDoList[] GetCategoryLists(int categoryId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from list in session.Query<ToDoList>()
                            where list.CategoryId == categoryId
                            select list;
                return query.ToArray();
            }
        }

        public static ToDoList GetListByCode(int code, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from list in session.Query<ToDoList>()
                            where list.UserCode == code && list.UserId == userId
                            select list;
                return query.FirstOrDefault();
            }
        }

        public static void AddList(ToDoList item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                item.UserCode = GetNextUserCode(item.UserId, session);
                CommonManager<ToDoList>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateList(ToDoList item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                CommonManager<ToDoList>.Update(item, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, ISession session)
        {
            var query = from item in session.Query<ToDoList>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }

        private static ToDoList ReadList(IDataReader reader)
        {
            ToDoList list = new ToDoList();
            list.Id = PgsqlUtils.GetInt("id", reader, 0);
            list.CategoryId = PgsqlUtils.GetInt("category_id", reader, 0);
            list.UserId = PgsqlUtils.GetInt("user_id", reader, 0);
            list.Name = PgsqlUtils.GetString("name", reader, "");
            list.Active = PgsqlUtils.GetBoolean("active", reader, false);
            list.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return list;
        }
    }
}

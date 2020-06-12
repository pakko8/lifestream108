using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LifeStream108.Modules.ToDoListManagement
{
    public static class ToDoCategoryManager
    {
        public static ToDoCategory[] GetUserCategories(int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from cat in session.Query<ToDoCategory>()
                            where cat.UserId == userId
                            select cat;
                return query.ToArray();
            }
        }

        public static ToDoCategory GetCategoryByCode(int categoryCode, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from cat in session.Query<ToDoCategory>()
                            where cat.UserCode == categoryCode && cat.UserId == userId
                            select cat;
                return query.FirstOrDefault();
            }
        }

        public static void AddCategory(ToDoCategory item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                item.UserCode = GetNextUserCode(item.UserId, session);
                CommonManager<ToDoCategory>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateCategory(ToDoCategory item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                CommonManager<ToDoCategory>.Update(item, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, DbConnection connection)
        {
            var query = from item in session.Query<ToDoCategory>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }

        private static ToDoCategory ReadCategory(IDataReader reader)
        {
            ToDoCategory cat = new ToDoCategory();
            cat.Id = PgsqlUtils.GetInt("Id", reader, 0);
            cat.UserId = PgsqlUtils.GetInt("user_id", reader, 0);
            cat.UserCode = PgsqlUtils.GetInt("user_code", reader, 0);
            cat.Name = PgsqlUtils.GetString("name", reader, "");
            cat.Email = PgsqlUtils.GetString("email", reader, "");
            cat.Active = PgsqlUtils.GetBoolean("active", reader, false);
            cat.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return cat;
        }
    }
}

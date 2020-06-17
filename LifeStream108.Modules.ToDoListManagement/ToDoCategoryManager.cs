using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LifeStream108.Modules.ToDoListManagement
{
    public static class ToDoCategoryManager
    {
        private const string TableName = "todo_list.todo_categories";

        public static ToDoCategory[] GetUserCategories(int userId)
        {
            List<ToDoCategory> cats = new List<ToDoCategory>();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName} where user_id={userId}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cats.Add(ReadCategory(reader));
                    }
                }
            }
            return cats.ToArray();
        }

        public static ToDoCategory GetCategoryByCode(int categoryCode, int userId)
        {
            ToDoCategory cat = null;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName} where user_id={userId} and user_code={categoryCode}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cat = ReadCategory(reader);
                    }
                }
            }
            return cat;
        }

        public static void AddCategory(ToDoCategory item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();

                item.UserCode = GetNextUserCode(item.UserId, connection);
                var command = connection.CreateCommand();
                command.CommandText =
                    $@"insert into {TableName}
                    (
                        user_id,
                        user_code,
                        name,
                        email,
                        active,
                        reg_time
                    )
                    values
                    (
                        @user_id,
                        @user_code,
                        @name,
                        @email,
                        @active,
                        @reg_time
                    )";
                command.Parameters.Add(new NpgsqlParameter("@user_id", DbType.Int32)).Value = item.UserId;
                command.Parameters.Add(new NpgsqlParameter("@user_code", DbType.Int32)).Value = item.UserCode;
                command.Parameters.Add(new NpgsqlParameter("@name", DbType.String)).Value = item.Name;
                command.Parameters.Add(new NpgsqlParameter("@active", DbType.Boolean)).Value = item.Active;
                command.Parameters.Add(new NpgsqlParameter("@reg_time", DbType.DateTime)).Value = item.RegTime;
                command.ExecuteNonQuery();
            }
        }

        public static void UpdateCategory(ToDoCategory item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    $@"update {TableName}
                    set
                        user_id=@user_id,
                        user_code=@user_code,
                        name=@name,
                        email=@email,
                        active=@active,
                        reg_time=@reg_time
                    where id=@id";
                command.Parameters.Add(new NpgsqlParameter("@id", DbType.Int32)).Value = item.Id;
                command.Parameters.Add(new NpgsqlParameter("@user_id", DbType.Int32)).Value = item.UserId;
                command.Parameters.Add(new NpgsqlParameter("@user_code", DbType.Int32)).Value = item.UserCode;
                command.Parameters.Add(new NpgsqlParameter("@name", DbType.String)).Value = item.Name;
                command.Parameters.Add(new NpgsqlParameter("@active", DbType.Boolean)).Value = item.Active;
                command.Parameters.Add(new NpgsqlParameter("@reg_time", DbType.DateTime)).Value = item.RegTime;
                command.ExecuteNonQuery();
            }
        }

        private static int GetNextUserCode(int userId, DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"select user_code from {TableName} where user_id={userId} order by user_code desc limit 1";
            int nextCode = 0;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    nextCode = PgsqlUtils.GetInt("user_code", reader, 0);
                }
            }
            return nextCode + 1;
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

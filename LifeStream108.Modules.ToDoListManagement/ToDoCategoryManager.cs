using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;

namespace LifeStream108.Modules.ToDoListManagement
{
    public static class ToDoCategoryManager
    {
        private const string TableName = "todo_list.todo_categories";

        public static ToDoCategory[] GetUserCategories(int userId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where user_id={userId}", ReadCategory);
        }

        public static ToDoCategory GetCategoryByCode(int categoryCode, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and user_code={categoryCode}", ReadCategory);
        }

        public static void AddCategory(ToDoCategory category)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                category.UserCode = GetNextUserCode(category.UserId, connection);

                string query =
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
                        current_timestamp
                    )
                    returning id";

                NpgsqlParameter[] parameters = new NpgsqlParameter[]
                {
                    PostgreSqlCommandUtils.CreateParam("@user_id", category.UserId, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@user_code", category.UserCode, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@name", category.Name, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@email", category.Email, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@active", category.Active, NpgsqlDbType.Boolean),
                };

                category.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);
            }
        }

        public static void UpdateCategory(ToDoCategory category)
        {
            string query =
                $@"update {TableName}
                set
                    user_code=@user_code,
                    name=@name,
                    email=@email,
                    active=@active
                where id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", category.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@user_code", category.UserCode, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@name", category.Name, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@email", category.Name, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@active", category.Active, NpgsqlDbType.Boolean),
            };

            PostgreSqlCommandUtils.UpdateEntity(query, parameters);
        }

        private static int GetNextUserCode(int userId, NpgsqlConnection connection)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select user_code from {TableName} where user_id={userId} order by user_code desc limit 1",
                connection, ReadUserCode);
        }

        private static int ReadUserCode(IDataReader reader)
        {
            return PgsqlUtils.GetInt("user_code", reader, 0) + 1;
        }

        private static ToDoCategory ReadCategory(IDataReader reader)
        {
            ToDoCategory cat = new ToDoCategory();
            cat.Id = PgsqlUtils.GetInt("Id", reader);
            cat.UserId = PgsqlUtils.GetInt("user_id", reader);
            cat.UserCode = PgsqlUtils.GetInt("user_code", reader);
            cat.Name = PgsqlUtils.GetString("name", reader);
            cat.Email = PgsqlUtils.GetString("email", reader);
            cat.Active = PgsqlUtils.GetBoolean("active", reader);
            cat.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return cat;
        }
    }
}

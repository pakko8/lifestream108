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
    public static class ToDoListManager
    {
        private const string TableName = "todo_list.todo_lists";

        public static ToDoList GetList(int listId)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where id={listId}", ReadList);
        }

        public static ToDoList[] GetUserLists(int userId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where user_id={userId}", ReadList);
        }

        public static ToDoList[] GetCategoryLists(int categoryId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where category_id={categoryId}", ReadList);
        }

        public static ToDoList GetListByCode(int userCode, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and user_code={userCode}", ReadList);
        }

        public static void AddList(ToDoList list)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();

                list.UserCode = GetNextUserCode(list.UserId, connection);

                string query =
                    $@"insert into {TableName}
                    (
                        user_id,
                        user_code,
                        category_id,
                        name,
                        active,
                        reg_time
                    )
                    values
                    (
                        @user_id,
                        @user_code,
                        @category_id,
                        @name,
                        @active,
                        current_timestamp
                    )
                    returning id";

                NpgsqlParameter[] parameters = new NpgsqlParameter[]
                {
                    PostgreSqlCommandUtils.CreateParam("@user_id", list.UserId, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@user_code", list.UserCode, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@category_id", list.CategoryId, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@name", list.Name, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@active", list.Active, NpgsqlDbType.Boolean),
                };

                list.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);
            }
        }

        public static void UpdateList(ToDoList list)
        {
            string query =
                $@"update {TableName}
                set
                    user_code=@user_code,
                    category_id=@category_id,
                    name=@name,
                    active=@active
                where id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", list.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@user_code", list.UserCode, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@category_id", list.CategoryId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@name", list.Name, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@active", list.Active, NpgsqlDbType.Boolean),
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

        private static ToDoList ReadList(IDataReader reader)
        {
            ToDoList list = new ToDoList();
            list.Id = PgsqlUtils.GetInt("id", reader);
            list.UserId = PgsqlUtils.GetInt("user_id", reader);
            list.UserCode = PgsqlUtils.GetInt("user_code", reader);
            list.CategoryId = PgsqlUtils.GetInt("category_id", reader);
            list.Name = PgsqlUtils.GetString("name", reader);
            list.Active = PgsqlUtils.GetBoolean("active", reader);
            list.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return list;
        }
    }
}

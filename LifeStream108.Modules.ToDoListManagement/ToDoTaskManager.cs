using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using System.Linq;

namespace LifeStream108.Modules.ToDoListManagement
{
    public static class ToDoTaskManager
    {
        private const string TableName = "todo_list.todo_tasks";

        public static ToDoTask GetTask(int taskId)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where id={taskId}", ReadTask);
        }

        public static ToDoTask GetTaskByTitle(string title, int listId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where list_id={listId} and status<>{(int)ToDoTaskStatus.Deleted} and upper(title)={title.Trim().ToUpper()}", ReadTask);
        }

        public static ToDoTask[] GetListActiveTasks(int listId)
        {
            return PostgreSqlCommandUtils.GetEntities(
                $"select * from {TableName} where list_id={listId} and status<>{(int)ToDoTaskStatus.Deleted}", ReadTask);
        }

        public static ToDoTask[] GetTasksWithActiveReminders(int userId)
        {
            return PostgreSqlCommandUtils.GetEntities(
                $"select * from {TableName} where user_id={userId} and status<>{(int)ToDoTaskStatus.Deleted} and reminder_sett<>''", ReadTask);
        }

        public static ToDoTask[] FindTasks(string word, int limit, int userId)
        {
            return PostgreSqlCommandUtils.GetEntities(
                $"select * from {TableName} where user_id={userId} and status<>{(int)ToDoTaskStatus.Deleted} and upper(title) like %{word.ToUpper()}% limit {limit}", ReadTask);
        }

        public static void AddTask(ToDoTask task)
        {
            string query =
                $@"insert into {TableName}
                (
                    sort_order,
                    list_id,
                    user_id,
                    title,
                    note,
                    file,
                    status,
                    reg_time,
                    content_update_time,
                    status_update_time,
                    reminder_sett,
                    reminder_last_time,
                    repetitive
                )
                values
                (
                    @sort_order,
                    @list_id,
                    @user_id,
                    @title,
                    @note,
                    @file,
                    @status,
                    current_timestamp,
                    current_timestamp,
                    @status_update_time,
                    @reminder_sett,
                    @reminder_last_time,
                    @repetitive
                )
                returning id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@sort_order", task.SortOrder, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@list_id", task.ListId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@user_id", task.UserId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@title", task.Title, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@note", task.Note, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@file", task.Files, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@status", (int)task.Status, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@status_update_time", DateTime.MinValue, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@reminder_sett", task.ReminderSettings, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@reminder_last_time", DateTime.MinValue, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@repetitive", task.IsRepetitive, NpgsqlDbType.Bit),
            };

            task.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);
        }

        public static void UpdateTask(ToDoTask task)
        {
            string query =
                $@"update {TableName}
                set
                    sort_order=@sort_order,
                    list_id=@list_id,
                    title=@title,
                    note=@note,
                    file=@file,
                    status=@status,
                    content_update_time=current_timestamp,
                    status_update_time=current_timestamp,
                    reminder_sett@reminder_sett,
                    reminder_last_time=@reminder_last_time,
                    repetitive=@repetitive
                where id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", task.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@sort_order", task.SortOrder, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@list_id", task.ListId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@title", task.Title, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@note", task.Note, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@file", task.Files, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@status", (int)task.Status, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@content_update_time", DateTime.Now, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@status_update_time", DateTime.Now, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@reminder_sett", task.ReminderSettings, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@reminder_last_time", DateTime.MinValue, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@repetitive", task.IsRepetitive, NpgsqlDbType.Bit),
            };

            PostgreSqlCommandUtils.UpdateEntity(query, parameters);
        }

        public static ToDoTask ReadTask(IDataReader reader)
        {
            ToDoTask task = new ToDoTask();
            task.Id = PgsqlUtils.GetInt("id", reader);
            task.SortOrder = PgsqlUtils.GetInt("sort_order", reader);
            task.ListId = PgsqlUtils.GetInt("list_id", reader);
            task.UserId = PgsqlUtils.GetInt("user_id", reader);
            task.Title = PgsqlUtils.GetString("title", reader);
            task.Note = PgsqlUtils.GetString("note", reader);
            task.Files = PgsqlUtils.GetString("files", reader);
            task.Status = (ToDoTaskStatus)PgsqlUtils.GetEnum("status", reader, typeof(ToDoTaskStatus), ToDoTaskStatus.New);
            task.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            task.ContentUpdateTime = PgsqlUtils.GetDateTime("content_update_time", reader, DateTime.MinValue);
            task.StatusUpdateTime = PgsqlUtils.GetDateTime("status_update_time", reader, DateTime.MinValue);
            task.ReminderSettings = PgsqlUtils.GetString("reminder_sett", reader);
            task.ReminderLastTime = PgsqlUtils.GetDateTime("reminder_last_time", reader, DateTime.MinValue);
            task.IsRepetitive = PgsqlUtils.GetBoolean("repetitive", reader);
            return task;
        }
    }
}

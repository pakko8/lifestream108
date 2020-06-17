using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
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
            ToDoTask task = null;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName} where id={taskId}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        task = ReadTask(reader);
                    }
                }
            }
            return task;
        }

        public static ToDoTask GetTaskByTitle(string title, int listId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value)) using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from task in session.Query<ToDoTask>()
                            where task.Title == title.Trim() && task.ListId == listId && task.Status != ToDoTaskStatus.Deleted
                            select task;
                return query.FirstOrDefault();
            }
        }

        public static ToDoTask[] GetListActiveTasks(int listId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from task in session.Query<ToDoTask>()
                            where task.ListId == listId && task.Status != ToDoTaskStatus.Deleted
                            select task;
                return query.ToArray();
            }
        }

        public static ToDoTask[] GetTasksWithActiveReminders(int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from task in session.Query<ToDoTask>()
                            where task.UserId == userId && task.ReminderSettings != ""
                                && task.Status != ToDoTaskStatus.Deleted
                            select task;
                return query.ToArray();
            }
        }

        public static ToDoTask[] FindTasks(string word, int limit, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from task in session.Query<ToDoTask>()
                            where task.UserId == userId && task.Status != ToDoTaskStatus.Deleted && task.Title.ToUpper().Contains(word.ToUpper())
                            select task;
                return query.Take(limit).ToArray();
            }
        }

        public static void AddTask(ToDoTask item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                CommonManager<ToDoTask>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateTask(ToDoTask item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                CommonManager<ToDoTask>.Update(item, session);
                session.Flush();
            }
        }

        public static ToDoTask ReadTask(IDataReader reader)
        {
            ToDoTask task = new ToDoTask();
            task.Id = PgsqlUtils.GetInt("id", reader, 0);
            task.SortOrder = PgsqlUtils.GetInt("sort_order", reader, 0);
            task.ListId = PgsqlUtils.GetInt("list_id", reader, 0);
            task.UserId = PgsqlUtils.GetInt("user_id", reader, 0);
            task.Title = PgsqlUtils.GetString("title", reader, "");
            task.Note = PgsqlUtils.GetString("note", reader, "");
            task.Files = PgsqlUtils.GetString("files", reader, "");
            task.Status = (ToDoTaskStatus)PgsqlUtils.GetEnum("status", reader, typeof(ToDoTaskStatus), ToDoTaskStatus.New);
            task.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            task.ContentUpdateTime = PgsqlUtils.GetDateTime("content_update_time", reader, DateTime.MinValue);
            task.StatusUpdateTime = PgsqlUtils.GetDateTime("status_update_time", reader, DateTime.MinValue);
            task.ReminderSettings = PgsqlUtils.GetDateTime("reminder_sett", reader, "");
            task.ReminderLastTime = PgsqlUtils.GetDateTime("reminder_last_time", reader, DateTime.MinValue);
            task.IsRepetitive = PgsqlUtils.GetBoolean("repetitive", reader, false);
            return task;
        }
    }
}

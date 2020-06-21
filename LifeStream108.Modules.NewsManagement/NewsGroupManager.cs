using System;
using System.Data;
using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.NewsEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using Npgsql;
using NpgsqlTypes;

namespace LifeStream108.Modules.NewsManagement
{
    public static class NewsGroupManager
    {
        private const string TableName = "news.news_groups";

        public static NewsGroup[] GetAllActiveGroups()
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName}", ReadGroup);
        }

        public static void UpdateGroup(NewsGroup group)
        {
            string query =
                $@"update {TableName}
                set
                    priority=@priority,
                    name=@name,
                    description=@description,
                    url=@url,
                    processor_class_name=@processor_class_name,
                    active=@active,
                    reg_time=@reg_time,
                    check_interval_in_minutes=@check_interval_in_minutes,
                    last_run_time=@last_run_time,
                    run_status=@run_status
                where
                    id={group.Id}";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("priority", group.Priority, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("name", group.Name, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("description", group.Description, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("url", group.Url, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("processor_class_name", group.ProcessorClassName, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("active", group.Active, NpgsqlDbType.Boolean),
                PostgreSqlCommandUtils.CreateParam("reg_time", group.RegTime, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("check_interval_in_minutes", group.CheckIntervalInMinutes, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("last_run_time", group.LastRunTime, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("run_status", group.RunStatus.ToString(), NpgsqlDbType.Varchar)
            };

            PostgreSqlCommandUtils.UpdateEntity(query, parameters);
        }

        private static NewsGroup ReadGroup(IDataReader reader)
        {
            NewsGroup group = new NewsGroup();
            group.Id = PgsqlUtils.GetInt("id", reader);
            group.Priority = PgsqlUtils.GetInt("priority", reader);
            group.Name = PgsqlUtils.GetString("name", reader);
            group.Description = PgsqlUtils.GetString("description", reader);
            group.Url = PgsqlUtils.GetString("url", reader);
            group.ProcessorClassName = PgsqlUtils.GetString("processor_class_name", reader);
            group.Active = PgsqlUtils.GetBoolean("active", reader);
            group.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            group.CheckIntervalInMinutes = PgsqlUtils.GetInt("check_interval_in_minutes", reader, 10);
            group.LastRunTime = PgsqlUtils.GetDateTime("last_run_time", reader, DateTime.MinValue);
            group.RunStatus = (NewsGroupRunStatus)PgsqlUtils.GetEnum("run_status", reader, typeof(NewsGroupRunStatus), NewsGroupRunStatus.Error);
            return group;
        }
    }
}

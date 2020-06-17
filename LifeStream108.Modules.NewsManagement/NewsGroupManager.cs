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
                where id={group.Id}";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("priority", NpgsqlDbType.Integer, group.Priority),
                PostgreSqlCommandUtils.CreateParam("name", NpgsqlDbType.Varchar, group.Name),
                PostgreSqlCommandUtils.CreateParam("description", NpgsqlDbType.Varchar, group.Description),
                PostgreSqlCommandUtils.CreateParam("url", NpgsqlDbType.Varchar, group.Url),
                PostgreSqlCommandUtils.CreateParam("processor_class_name", NpgsqlDbType.Varchar, group.ProcessorClassName),
                PostgreSqlCommandUtils.CreateParam("active", NpgsqlDbType.Boolean, group.Active),
                PostgreSqlCommandUtils.CreateParam("reg_time", NpgsqlDbType.Timestamp, group.RegTime),
                PostgreSqlCommandUtils.CreateParam("check_interval_in_minutes", NpgsqlDbType.Integer, group.CheckIntervalInMinutes),
                PostgreSqlCommandUtils.CreateParam("last_run_time", NpgsqlDbType.Timestamp, group.LastRunTime),
                PostgreSqlCommandUtils.CreateParam("run_status", NpgsqlDbType.Varchar, group.RunStatus.ToString())
            };

            PostgreSqlCommandUtils.UpdateEntity(query, parameters);
        }

        private static NewsGroup ReadGroup(IDataReader reader)
        {
            NewsGroup group = new NewsGroup();
            group.Id = PgsqlUtils.GetInt("id", reader, 0);
            group.Priority = PgsqlUtils.GetInt("priority", reader, 0);
            group.Name = PgsqlUtils.GetString("name", reader, "");
            group.Description = PgsqlUtils.GetString("description", reader, "");
            group.Url = PgsqlUtils.GetString("url", reader, "");
            group.ProcessorClassName = PgsqlUtils.GetString("processor_class_name", reader, "");
            group.Active = PgsqlUtils.GetBoolean("active", reader, false);
            group.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            group.CheckIntervalInMinutes = PgsqlUtils.GetInt("check_interval_in_minutes", reader, 10);
            group.LastRunTime = PgsqlUtils.GetDateTime("last_run_time", reader, DateTime.MinValue);
            group.RunStatus = (NewsGroupRunStatus)PgsqlUtils.GetEnum("run_status", reader, typeof(NewsGroupRunStatus), NewsGroupRunStatus.Error);
            return group;
        }
    }
}

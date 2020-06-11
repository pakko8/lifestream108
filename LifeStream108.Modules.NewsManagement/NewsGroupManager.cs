using System;
using System.Collections.Generic;
using System.Data;
using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.NewsEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using NpgsqlTypes;

namespace LifeStream108.Modules.NewsManagement
{
    public static class NewsGroupManager
    {
        private const string TableName = "news.news_groups";

        public static NewsGroup[] GetAllActiveGroups()
        {
            List<NewsGroup> groups = new List<NewsGroup>();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(ReadGroup(reader));
                    }
                }
            }
            return groups.ToArray();
        }

        public static void UpdateGroup(NewsGroup group)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText =
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
                command.Parameters.Add("priority", NpgsqlDbType.Integer).Value = group.Priority;
                command.Parameters.Add("name", NpgsqlDbType.Varchar).Value = group.Name;
                command.Parameters.Add("description", NpgsqlDbType.Varchar).Value = group.Description;
                command.Parameters.Add("url", NpgsqlDbType.Varchar).Value = group.Url;
                command.Parameters.Add("processor_class_name", NpgsqlDbType.Varchar).Value = group.ProcessorClassName;
                command.Parameters.Add("active", NpgsqlDbType.Boolean).Value = group.Active;
                command.Parameters.Add("reg_time", NpgsqlDbType.Timestamp).Value = group.RegTime;
                command.Parameters.Add("check_interval_in_minutes", NpgsqlDbType.Integer).Value = group.CheckIntervalInMinutes;
                command.Parameters.Add("last_run_time", NpgsqlDbType.Timestamp).Value = group.LastRunTime;
                command.Parameters.Add("run_status", NpgsqlDbType.Varchar).Value = group.RunStatus.ToString();
                connection.Open();
            }
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

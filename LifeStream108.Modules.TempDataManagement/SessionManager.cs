using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace LifeStream108.Modules.TempDataManagement
{
    public static class SessionManager
    {
        private const string TableName = "temp_data.sessions";

        public static Session[] GetAllSessions()
        {
            List<Session> sessions = new List<Session>();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sessions.Add(ReadSession(reader));
                    }
                }
            }
            return sessions.ToArray();
        }

        public static Session GetSessionForUser(int userId)
        {
            Session session = null;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName} where user_id={userId}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        session = ReadSession(reader);
                    }
                }
            }
            return session;
        }

        public static void AddSession(Session item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    $@"insert into {TableName}
                    (
                        user_id,
                        project_id,
                        last_command_id,
                        last_life_group_id,
                        last_life_activity_id,
                        last_request_text,
                        data, start_time,
                        last_activity_time
                    )
                    values
                    (
                        @user_id,
                        @project_id,
                        @last_command_id,
                        @last_life_group_id,
                        @last_life_activity_id,
                        @last_request_text,
                        @data,
                        current_timestamp,
                        current_timestamp
                    )";
                command.Parameters.Add(new NpgsqlParameter("@user_id", DbType.Int32)).Value = item.UserId;
                command.Parameters.Add(new NpgsqlParameter("@project_id", DbType.Int32)).Value = item.ProjectId;
                command.Parameters.Add(new NpgsqlParameter("@last_command_id", DbType.Int32)).Value = item.LastCommandId;
                command.Parameters.Add(new NpgsqlParameter("@last_life_group_id", DbType.Int32)).Value = item.LastLifeGroupId;
                command.Parameters.Add(new NpgsqlParameter("@last_life_activity_id", DbType.Int32)).Value = item.LastLifeActivityId;
                command.Parameters.Add(new NpgsqlParameter("@last_request_text", DbType.String)).Value = item.LastRequestText;
                command.Parameters.Add(new NpgsqlParameter("@data", DbType.String)).Value = item.Data;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void UpdateSession(Session item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    $@"update {TableName}
                    set
                        user_id=@user_id,
                        project_id=@project_id,
                        last_command_id=@last_command_id,
                        last_life_group_id=@last_life_group_id,
                        last_life_activity_id=@last_life_activity_id,
                        last_request_text=@last_request_text,
                        data=@data,
                        last_activity_time=current_timestamp
                    where
                        id=@id";
                command.Parameters.Add(new NpgsqlParameter("@id", DbType.Int32)).Value = item.Id;
                command.Parameters.Add(new NpgsqlParameter("@user_id", DbType.Int32)).Value = item.UserId;
                command.Parameters.Add(new NpgsqlParameter("@project_id", DbType.Int32)).Value = item.ProjectId;
                command.Parameters.Add(new NpgsqlParameter("@last_command_id", DbType.Int32)).Value = item.LastCommandId;
                command.Parameters.Add(new NpgsqlParameter("@last_life_group_id", DbType.Int32)).Value = item.LastLifeGroupId;
                command.Parameters.Add(new NpgsqlParameter("@last_life_activity_id", DbType.Int32)).Value = item.LastLifeActivityId;
                command.Parameters.Add(new NpgsqlParameter("@last_request_text", DbType.String)).Value = item.LastRequestText;
                command.Parameters.Add(new NpgsqlParameter("@data", DbType.String)).Value = item.Data;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Delete(Session item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"delete from {TableName} where id={item.Id}";
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static Session ReadSession(IDataReader reader)
        {
            Session session = new Session();
            session.Id = PgsqlUtils.GetInt("id", reader);
            session.UserId = PgsqlUtils.GetInt("user_id", reader);
            session.ProjectId = PgsqlUtils.GetInt("project_id", reader);
            session.LastCommandId = PgsqlUtils.GetInt("last_command_id", reader, 0);
            session.LastLifeGroupId = PgsqlUtils.GetInt("last_life_group_id", reader);
            session.LastLifeActivityId = PgsqlUtils.GetInt("last_life_activity_id", reader);
            session.LastRequestText = PgsqlUtils.GetString("last_request_text", reader, "");
            session.Data = PgsqlUtils.GetString("data", reader, "");
            session.StartTime = PgsqlUtils.GetDateTime("start_time", reader, DateTime.Now);
            session.LastActivityTime = PgsqlUtils.GetDateTime("last_activity_time", reader, DateTime.Now);
            return session;
        }
    }
}

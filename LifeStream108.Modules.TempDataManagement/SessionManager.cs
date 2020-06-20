using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;

namespace LifeStream108.Modules.TempDataManagement
{
    public static class SessionManager
    {
        private const string TableName = "temp_data.sessions";

        public static Session[] GetAllSessions()
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName}", ReadSession);
        }

        public static Session GetSessionForUser(int userId)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where user_id={userId}", ReadSession);
        }

        public static void AddSession(Session item)
        {
            string query =
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
                )
                returning id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@user_id", item.UserId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@project_id", item.ProjectId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@last_command_id", item.LastCommandId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@last_life_group_id", item.LastLifeGroupId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@last_life_activity_id", item.LastLifeActivityId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@last_request_text", item.LastRequestText, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@data", item.Data, NpgsqlDbType.Varchar)
            };

            item.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);
        }

        public static void UpdateSession(Session item)
        {
            string query =
                $@"update {TableName}
                set
                    project_id=@project_id,
                    last_command_id=@last_command_id,
                    last_life_group_id=@last_life_group_id,
                    last_life_activity_id=@last_life_activity_id,
                    last_request_text=@last_request_text,
                    data=@data,
                    last_activity_time=current_timestamp
                where
                    id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", item.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@project_id", item.ProjectId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@last_command_id", item.LastCommandId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@last_life_group_id", item.LastLifeGroupId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@last_life_activity_id", item.LastLifeActivityId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@last_request_text", item.LastRequestText, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@data", item.Data, NpgsqlDbType.Varchar)
            };

            PostgreSqlCommandUtils.UpdateEntity(query, parameters);
        }

        public static void Delete(Session item)
        {
            PostgreSqlCommandUtils.UpdateEntity($"delete from {TableName} where id={item.Id}", null);
        }

        private static Session ReadSession(IDataReader reader)
        {
            Session session = new Session();
            session.Id = PgsqlUtils.GetInt("id", reader);
            session.UserId = PgsqlUtils.GetInt("user_id", reader);
            session.ProjectId = PgsqlUtils.GetInt("project_id", reader);
            session.LastCommandId = PgsqlUtils.GetInt("last_command_id", reader);
            session.LastLifeGroupId = PgsqlUtils.GetInt("last_life_group_id", reader);
            session.LastLifeActivityId = PgsqlUtils.GetInt("last_life_activity_id", reader);
            session.LastRequestText = PgsqlUtils.GetString("last_request_text", reader);
            session.Data = PgsqlUtils.GetString("data", reader);
            session.StartTime = PgsqlUtils.GetDateTime("start_time", reader, DateTime.Now);
            session.LastActivityTime = PgsqlUtils.GetDateTime("last_activity_time", reader, DateTime.Now);
            return session;
        }
    }
}

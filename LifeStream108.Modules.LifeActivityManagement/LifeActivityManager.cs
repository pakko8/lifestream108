using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeActivityManager
    {
        private const string TableName = "activities.life_activities";

        public static (LifeActivity Activity, LifeActivityParameter[] Parameters) GetActivityWithParams(int activityId, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();

                LifeActivity activity = PostgreSqlCommandUtils.GetEntity(
                    $"select * from {TableName} where user_id={userId} and id={activityId}", ReadActivity, connection);

                LifeActivityParameter[] parameters = null;
                if (activity != null)
                {
                    parameters = LifeActivityParameterManager.GetParametersByActivity(activity.Id, connection, true);
                }
                return (activity, parameters);
            }
        }

        public static LifeActivity[] GetActivitiesForUser(int userId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where user_id={userId}", ReadActivity);
        }

        public static LifeActivity GetActivityByUserCode(int userCode, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and user_code={userCode}", ReadActivity);
        }

        public static (LifeActivity Activity, LifeActivityParameter[] Parameters) GetActivityAndParamsByUserCode(int userCode, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();

                LifeActivity activity = GetActivityByUserCode(userCode, userId, connection);
                LifeActivityParameter[] parameters = null;
                if (activity != null)
                {
                    parameters = LifeActivityParameterManager.GetParametersByActivity(activity.Id, connection, true);
                }
                return (activity, parameters);
            }
        }

        private static LifeActivity GetActivityByUserCode(int userCode, int userId, NpgsqlConnection connection)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and user_code={userCode}", ReadActivity);
        }

        public static LifeActivity GetActivityByName(string activityName, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and upper(name)='{activityName.ToUpper()}'", ReadActivity);
        }

        public static LifeActivity[] FindActivitiesByName(string word, int userId)
        {
            return PostgreSqlCommandUtils.GetEntities(
                $"select * from {TableName} where user_id={userId} and upper(name) like %{word.ToUpper()}%", ReadActivity);
        }

        public static void AddActivity(LifeActivity activity)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();

                activity.UserCode = GetNextUserCode(activity.UserId, connection);

                string query =
                    $@"insert into {TableName}
                    (
                        user_code,
                        user_id,
                        name,
                        short_name,
                        type,
                        life_group_at_group_id,
                        active,
                        reg_time
                    )
                    values
                    (
                        @user_code,
                        @user_id,
                        @name,
                        @short_name,
                        @type,
                        @life_group_at_group_id,
                        @active,
                        current_timestamp
                    )
                    returning id";

                NpgsqlParameter[] parameters = new NpgsqlParameter[]
                {
                    PostgreSqlCommandUtils.CreateParam("@user_code", activity.UserCode, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@user_id", activity.UserId, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@name", activity.Name, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@short_name", activity.ShortName, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@type", (int)activity.PeriodType, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@life_group_at_group_id", activity.LifeGroupAtGroupId, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@active", activity.Active, NpgsqlDbType.Boolean),
                };

                activity.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);
            }
        }

        public static void UpdateActivity(LifeActivity activity)
        {
            string query =
                $@"update {TableName}
                set
                    user_code=@user_code,
                    name=@name,
                    short_name=@short_name,
                    type=@type,
                    life_group_at_group_id=@life_group_at_group_id,
                    active=@active
                where
                    id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", activity.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@user_code", activity.UserCode, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@name", activity.Name, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@short_name", activity.ShortName, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@type", (int)activity.PeriodType, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@life_group_at_group_id", activity.LifeGroupAtGroupId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@active", activity.Active, NpgsqlDbType.Boolean),
            };

            PostgreSqlCommandUtils.UpdateEntity(query, parameters);
        }

        private static int GetNextUserCode(int userId, NpgsqlConnection connection)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select user_code from {TableName} where user_id={userId} order by user_code desc limit 1",
                ReadUserCode, connection);
        }

        private static int ReadUserCode(IDataReader reader)
        {
            return PgsqlUtils.GetInt("user_code", reader, 0) + 1;
        }

        private static LifeActivity ReadActivity(IDataReader reader)
        {
            LifeActivity act = new LifeActivity();
            act.Id = PgsqlUtils.GetInt("id", reader);
            act.UserCode = PgsqlUtils.GetInt("user_code", reader);
            act.UserId = PgsqlUtils.GetInt("user_id", reader);
            act.Name = PgsqlUtils.GetString("name", reader);
            act.ShortName = PgsqlUtils.GetString("short_name", reader);
            act.PeriodType = (PeriodicityType)PgsqlUtils.GetEnum("type", reader, typeof(PeriodicityType), PeriodicityType.None);
            act.LifeGroupAtGroupId = PgsqlUtils.GetInt("life_group_at_group_id", reader);
            act.Active = PgsqlUtils.GetBoolean("active", reader);
            act.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return act;
        }
    }
}

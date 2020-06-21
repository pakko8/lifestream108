using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeActivityParameterManager
    {
        private const string TableName = "activities.life_activity_params";

        public static LifeActivityParameter GetParameterByCode(int userCode, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and user_code={userCode}", ReadParameter);
        }

        public static LifeActivityParameter GetParameterByName(string paramName, int activityId, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and activity_id={activityId} and upper(name)={paramName.ToUpper()}",
                ReadParameter);
        }

        public static LifeActivityParameter[] GetParametersForUser(int userId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where user_id={userId}", ReadParameter);
        }

        internal static LifeActivityParameter[] GetParametersByActivity(
            int activityId, NpgsqlConnection connection, bool onlyActive = true)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where activity_id={activityId}",
                ReadParameter, connection);
        }

        public static void AddParameters(LifeActivityParameter[] parameters)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                int userCode = GetNextUserCode(parameters[0].UserId, connection);
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (LifeActivityParameter param in parameters)
                        {
                            param.UserCode = userCode++;
                            string query =
                                $@"insert into {TableName}
                                (
                                    sort_order,
                                    user_code,
                                    user_id,
                                    activity_id,
                                    name,
                                    measure_id,
                                    data_type,
                                    function,
                                    active,
                                    reg_time
                                )
                                values
                                (
                                    @sort_order,
                                    @user_code,
                                    @user_id,
                                    @activity_id,
                                    @name,
                                    @measure_id,
                                    @data_type,
                                    @function,
                                    @active,
                                    current_timestamp
                                )
                                returning id";

                                NpgsqlParameter[] dbParams = new NpgsqlParameter[]
                                {
                                    PostgreSqlCommandUtils.CreateParam("@id", param.Id, NpgsqlDbType.Integer),
                                    PostgreSqlCommandUtils.CreateParam("@user_code", param.UserCode, NpgsqlDbType.Integer),
                                    PostgreSqlCommandUtils.CreateParam("@user_id", param.UserId, NpgsqlDbType.Integer),
                                    PostgreSqlCommandUtils.CreateParam("@activity_id", param.ActivityId, NpgsqlDbType.Integer),
                                    PostgreSqlCommandUtils.CreateParam("@name", param.Name, NpgsqlDbType.Varchar),
                                    PostgreSqlCommandUtils.CreateParam("@measure_id", param.MeasureId, NpgsqlDbType.Integer),
                                    PostgreSqlCommandUtils.CreateParam("@data_type", param.DataType, NpgsqlDbType.Varchar),
                                    PostgreSqlCommandUtils.CreateParam("@function", param.Fuction, NpgsqlDbType.Varchar),
                                    PostgreSqlCommandUtils.CreateParam("@active", param.Active, NpgsqlDbType.Boolean),
                                };

                            param.Id = PostgreSqlCommandUtils.AddEntity<int>(query, dbParams, connection);

                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public static void UpdateParameter(LifeActivityParameter param)
        {
            string query =
                $@"update {TableName}
                set
                    sort_order=@sort_order,
                    user_code=@user_code,
                    activity_id=@activity_id,
                    name=@name,
                    measure_id=@measure_id,
                    data_type=@data_type,
                    function=@function,
                    active=@active
                where
                    id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", param.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@user_code", param.UserCode, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@user_id", param.UserId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@activity_id", param.ActivityId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@name", param.Name, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@measure_id", param.MeasureId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@data_type", param.DataType, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@function", param.Fuction, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@active", param.Active, NpgsqlDbType.Boolean),
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

        private static LifeActivityParameter ReadParameter(IDataReader reader)
        {
            LifeActivityParameter param = new LifeActivityParameter();
            param.Id = PgsqlUtils.GetInt("id", reader);
            param.SortOrder = PgsqlUtils.GetInt("sort_order", reader);
            param.UserCode = PgsqlUtils.GetInt("user_code", reader);
            param.UserId = PgsqlUtils.GetInt("user_id", reader);
            param.ActivityId = PgsqlUtils.GetInt("activity_id", reader);
            param.Name = PgsqlUtils.GetString("name", reader);
            param.MeasureId = PgsqlUtils.GetInt("measure_id", reader);
            param.DataType = (DataType)PgsqlUtils.GetEnum("data_type", reader, typeof(DataType), DataType.Text);
            param.Fuction = PgsqlUtils.GetString("function", reader);
            param.Active = PgsqlUtils.GetBoolean("active", reader);
            param.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.Now);
            return param;
        }
    }
}

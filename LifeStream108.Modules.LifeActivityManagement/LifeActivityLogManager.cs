using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.SettingsManagement;
using NLog;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeActivityLogManager
    {
        private const string TableNameLogs = "life_activity_logs.life_activity_logs";
        private const string TableNameLogValues = "life_activity_logs.life_activity_log_values";

        public static LifeActivityLog[] GetLogsForPeriod(DateTime dateFrom, DateTime dateTo, int userId)
        {
            string query = $"select * from {TableNameLogs} where user_id={userId} and period>=@periodFrom and period<@periodTo";
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@periodFrom", DateUtils.GetBeginOfDay(dateFrom), NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@periodTo", DateUtils.GetBeginOfNextDay(dateTo), NpgsqlDbType.Timestamp)
            };
            return PostgreSqlCommandUtils.GetEntities(query, ReadLog, parameters);
        }

        public static LifeActivityLog[] GetLogsForPeriod(DateTime dateFrom, DateTime dateTo, int userId, int[] activityIds)
        {
            string query =
                $"select * from {TableNameLogs} where user_id={userId} and period>=@periodFrom and period<@periodTo and life_activity_id in ({CollectionUtils.Array2String<int>(activityIds)})";
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@periodFrom", DateUtils.GetBeginOfDay(dateFrom), NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@periodTo", DateUtils.GetBeginOfNextDay(dateTo), NpgsqlDbType.Timestamp)
            };
            return PostgreSqlCommandUtils.GetEntities(query, ReadLog, parameters);
        }

        public static LifeActivityLogValue[] GetLogValuesForPeriod(DateTime dateFrom, DateTime dateTo, int userId)
        {
            string query =
                $@"select val.* from {TableNameLogValues} val
                inner join {TableNameLogs} log on log.id=val.activity_log_id
                where log.active='t' and val.user_id={userId} 
                and val.period>=@periodFrom and val.period<=@periodTo";
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@periodFrom", dateFrom, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@periodTo", dateTo, NpgsqlDbType.Timestamp)
            };
            return PostgreSqlCommandUtils.GetEntities(query, ReadLogValue, parameters);
        }

        public static LifeActivityLogWithValues[] GetLogsForDate(
            int activityId, DateTime date, int userId, bool onlyActive = true)
        {
            List<LifeActivityLogWithValues> logList = new List<LifeActivityLogWithValues>();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();

                string query = $"select * from {TableNameLogs} where user_id={userId} and life_activity_id={activityId} and period>=@dtFrom and period<@dtTo";
                if (onlyActive) query += " and active='t'";
                NpgsqlParameter[] parameters = new NpgsqlParameter[]
                {
                    PostgreSqlCommandUtils.CreateParam("@dtFrom", DateUtils.GetBeginOfDay(date), NpgsqlDbType.Date),
                    PostgreSqlCommandUtils.CreateParam("@dtTo", DateUtils.GetBeginOfNextDay(date), NpgsqlDbType.Date)
                };
                LifeActivityLog[] logs = PostgreSqlCommandUtils.GetEntities(query, ReadLog, connection, parameters);
                foreach (LifeActivityLog log in logs)
                {
                    LifeActivityLogWithValues logWithValues = new LifeActivityLogWithValues();
                    logWithValues.Log = log;
                    logWithValues.Values = GetActivityLogValues(log.Id, connection);
                    logList.Add(logWithValues);
                }
            }
            return logList.ToArray();
        }

        public static LifeActivityLogWithValues GetLogWithValues(long logId, int userId)
        {
            LifeActivityLogWithValues logWithValues = new LifeActivityLogWithValues();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();

                logWithValues.Log = PostgreSqlCommandUtils.GetEntity(
                    $"select * from {TableNameLogs} where user_id={userId} and id={logId}", ReadLog, connection);
                logWithValues.Values = logWithValues.Log != null ? GetActivityLogValues(logWithValues.Log.Id, connection) : null;
            }
            return logWithValues;
        }

        private static LifeActivityLogValue[] GetActivityLogValues(long logId, NpgsqlConnection connection)
        {
            return PostgreSqlCommandUtils.GetEntities(
                $"select * from {TableNameLogValues} where activity_log_id={logId}", ReadLogValue, connection);
        }

        public static void AddLog(LifeActivityLog log, LifeActivityLogValue[] logValues)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        AddLog(log, connection);
                        foreach (LifeActivityLogValue logValue in logValues)
                        {
                            logValue.ActivityLogId = log.Id;
                            AddLogValue(logValue, connection);
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

        public static void UpdateLog(LifeActivityLog log)
        {
            UpdateLog(log, null as NpgsqlConnection);
        }

        public static void UpdateLog(LifeActivityLog log, LifeActivityLogValue[] logValues)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        UpdateLog(log, connection);
                        foreach (LifeActivityLogValue logValue in logValues)
                        {
                            UpdateLogValue(logValue, connection);
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

        private static void AddLog(LifeActivityLog log, NpgsqlConnection connection)
        {
            string query =
                $@"insert into {TableNameLogs}
                (
                    user_id,
                    life_activity_id,
                    period,
                    comment,
                    active,
                    reg_time,
                    update_time
                )
                values
                (
                    @user_id,
                    @life_activity_id,
                    @period,
                    @comment,
                    @active,
                    current_timestamp,
                    current_timestamp
                )
                returning id";
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@user_id", log.UserId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@life_activity_id", log.LifeActivityId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@period", log.Period.Date, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@comment", log.Comment, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@active", log.Active, NpgsqlDbType.Boolean)
            };
            log.Id = PostgreSqlCommandUtils.AddEntity<long>(query, parameters, connection);
        }

        private static void UpdateLog(LifeActivityLog log, NpgsqlConnection connection)
        {
            string query =
                $@"update {TableNameLogs}
                set
                    life_activity_id=@life_activity_id,
                    period=@period,
                    comment=@comment,
                    active=@active,
                    update_time=current_timestamp
                where
                    id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", log.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@life_activity_id", log.LifeActivityId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@period", log.Period.Date, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@comment", log.Comment, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@active", log.Active, NpgsqlDbType.Boolean)
            };

            if (connection == null)
                PostgreSqlCommandUtils.UpdateEntity(query, parameters);
            else
                PostgreSqlCommandUtils.UpdateEntity(query, parameters, connection);
        }

        private static void AddLogValue(LifeActivityLogValue logValue, NpgsqlConnection connection)
        {
            string query =
                $@"insert into {TableNameLogValues}
                (
                    user_id,
                    activity_log_id,
                    period,
                    activity_param_id,
                    numeric_value,
                    text_value
                )
                values
                (
                    @user_id,
                    @activity_log_id,
                    @period,
                    @activity_param_id,
                    @numeric_value,
                    @text_value
                )
                returning id";
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@user_id", logValue.UserId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@activity_log_id", logValue.ActivityLogId, NpgsqlDbType.Bigint),
                PostgreSqlCommandUtils.CreateParam("@period", logValue.Period.Date, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@activity_param_id", logValue.ActivityParamId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@numeric_value", logValue.NumericValue, NpgsqlDbType.Numeric),
                PostgreSqlCommandUtils.CreateParam("@text_value", logValue.TextValue, NpgsqlDbType.Varchar)
            };
            logValue.Id = PostgreSqlCommandUtils.AddEntity<long>(query, parameters, connection);
        }

        private static void UpdateLogValue(LifeActivityLogValue logValue, NpgsqlConnection connection)
        {
            string query =
                $@"update {TableNameLogValues}
                set
                    activity_log_id=@activity_log_id,
                    period=@period,
                    activity_param_id=@activity_param_id,
                    numeric_value=@numeric_value,
                    text_value=@text_value
                where
                    id=@id";
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", logValue.Id, NpgsqlDbType.Bigint),
                PostgreSqlCommandUtils.CreateParam("@activity_log_id", logValue.ActivityLogId, NpgsqlDbType.Bigint),
                PostgreSqlCommandUtils.CreateParam("@period", logValue.Period.Date, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@activity_param_id", logValue.ActivityParamId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@numeric_value", logValue.NumericValue, NpgsqlDbType.Numeric),
                PostgreSqlCommandUtils.CreateParam("@text_value", logValue.TextValue, NpgsqlDbType.Varchar)
            };
            PostgreSqlCommandUtils.UpdateEntity(query, parameters, connection);
        }

        private static LifeActivityLog ReadLog(IDataReader reader)
        {
            LifeActivityLog log = new LifeActivityLog();
            log.Id = PgsqlUtils.GetInt("id", reader);
            log.UserId = PgsqlUtils.GetInt("user_id", reader);
            log.LifeActivityId = PgsqlUtils.GetInt("life_activity_id", reader);
            log.Period = PgsqlUtils.GetDateTime("period", reader, DateTime.MinValue);
            log.Comment = PgsqlUtils.GetString("comment", reader);
            log.Active = PgsqlUtils.GetBoolean("active", reader);
            log.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            log.UpdateTime = PgsqlUtils.GetDateTime("update_time", reader, DateTime.MinValue);
            return log;
        }

        private static LifeActivityLogValue ReadLogValue(IDataReader reader)
        {
            LifeActivityLogValue value = new LifeActivityLogValue();
            value.Id = PgsqlUtils.GetInt("id", reader);
            value.UserId = PgsqlUtils.GetInt("user_id", reader);
            value.ActivityLogId = PgsqlUtils.GetLong("activity_log_id", reader);
            value.Period = PgsqlUtils.GetDateTime("period", reader, DateTime.MinValue);
            value.ActivityParamId = PgsqlUtils.GetInt("activity_param_id", reader);
            value.NumericValue = PgsqlUtils.GetDouble("numeric_value", reader);
            value.TextValue = PgsqlUtils.GetString("text_value", reader);
            return value;
        }
    }
}

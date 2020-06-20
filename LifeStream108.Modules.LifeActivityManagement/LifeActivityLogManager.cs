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
using System.Data.Common;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeActivityLogManager
    {
        private const string TableNameLogs = "life_activity_logs.life_activity_logs";
        private const string TableNameLogValues = "life_activity_logs.life_activity_log_values";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static LifeActivityLog[] GetLogsForPeriod(DateTime dateFrom, DateTime dateTo, int userId)
        {
            Logger.Info($"Getting activity logs for user={userId}, period={dateFrom:yyyy-MM-dd}-{dateTo:yyyy-MM-dd}");
            string query = $"select * {TableNameLogs} where user_id={userId} and period>=@periodFrom and period<=@periodTo";
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@periodFrom", dateFrom, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@periodTo", dateTo, NpgsqlDbType.Timestamp)
            };
            return PostgreSqlCommandUtils.GetEntities(query, ReadLog);
        }

        public static LifeActivityLog[] GetLogsForPeriod(DateTime dateFrom, DateTime dateTo, int userId, int[] activityIds)
        {
            Logger.Info($"Getting activity logs for user={userId}, period={dateFrom:yyyy-MM-dd}-{dateTo:yyyy-MM-dd}" +
                $"activityIds={CollectionUtils.Array2String<int>(activityIds)}");
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from log in session.Query<LifeActivityLog>()
                    where log.UserId == userId && activityIds.Contains(log.LifeActivityId) && log.Period >= dateFrom && log.Period <= dateTo
                    select log;
                return query.ToArray();
            }
        }

        public static LifeActivityLogValue[] GetLogValuesForPeriod(DateTime dateFrom, DateTime dateTo, int userId)
        {
            Logger.Info($"Getting activity log values for user={userId}, period={dateFrom:yyyy-MM-dd}-{dateTo:yyyy-MM-dd}");
            List<LifeActivityLogValue> values = new List<LifeActivityLogValue>();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                string commandText =
$@"select val.* from life_activity_logs.life_activity_log_values val
inner join life_activity_logs.life_activity_logs lg on lg.id=val.activity_log_id
where lg.active='t' and val.user_id={userId} 
and val.period>=timestamp'{dateFrom:yyyy-MM-dd}' and val.period<=timestamp'{dateTo:yyyy-MM-dd}'";
                DbCommand command = session.Connection.CreateCommand();
                command.CommandText = commandText;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        values.Add(ReadLogValue(reader));
                    }
                }
            }
            return values.ToArray();
        }

        public static LifeActivityLogWithValues[] GetLogsForDate(
            int activityId, DateTime date, int userId, bool onlyActive = true)
        {
            List<LifeActivityLogWithValues> logList = new List<LifeActivityLogWithValues>();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from log in session.Query<LifeActivityLog>()
                            where log.UserId == userId && log.LifeActivityId == activityId && log.Period == date
                            select log;
                LifeActivityLog[] logs = onlyActive ? query.Where(n => n.Active).ToArray() : query.ToArray();
                foreach (LifeActivityLog log in logs)
                {
                    LifeActivityLogWithValues logWithValues = new LifeActivityLogWithValues
                    {
                        Log = log
                    };
                    logWithValues.Values = GetActivityLogValues(log.Id, session);
                    logList.Add(logWithValues);
                }
            }
            return logList.ToArray();
        }

        public static LifeActivityLogWithValues GetLogWithValues(long logId, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from logItem in session.Query<LifeActivityLog>()
                            where logItem.UserId == userId && logItem.Id == logId
                            select logItem;
                LifeActivityLogWithValues logWithValues = new LifeActivityLogWithValues();
                logWithValues.Log = query.FirstOrDefault();
                logWithValues.Values = logWithValues.Log != null ? GetActivityLogValues(logWithValues.Log.Id, session) : null;

                return logWithValues;
            }
        }

        private static LifeActivityLogValue[] GetActivityLogValues(long logId, ISession session)
        {
            var query = from logValue in session.Query<LifeActivityLogValue>()
                        where logValue.ActivityLogId == logId
                        select logValue;
            return query.ToArray();
        }

        public static void AddLog(LifeActivityLog log, LifeActivityLogValue[] logValues)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        CommonManager<LifeActivityLog>.Add(log, session);
                        foreach (LifeActivityLogValue logValue in logValues)
                        {
                            logValue.ActivityLogId = log.Id;
                            CommonManager<LifeActivityLogValue>.Add(logValue, session);
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

        private static void UpdateLog(LifeActivityLog log)
        {
            UpdateLog(log, null as NpgsqlConnection);
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
                where id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", log.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@life_activity_id", log.LifeActivityId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@period", log.Period, NpgsqlDbType.Timestamp),
                PostgreSqlCommandUtils.CreateParam("@comment", log.Comment, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@active", log.Active, NpgsqlDbType.Boolean)
            };

            if (connection == null)
                PostgreSqlCommandUtils.UpdateEntity(query, parameters);
            else
                PostgreSqlCommandUtils.UpdateEntity(query, parameters, connection);
        }

        public static void UpdateLog(LifeActivityLog log, LifeActivityLogValue[] logValues)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        log.UpdateTime = DateTime.Now;
                        CommonManager<LifeActivityLog>.Update(log, session);
                        foreach (LifeActivityLogValue logValue in logValues)
                        {
                            CommonManager<LifeActivityLogValue>.Update(logValue, session);
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

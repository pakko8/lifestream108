using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement.Managers
{
    public static class LifeActivityLogManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static LifeActivityLog[] GetLogsForPeriod(DateTime dateFrom, DateTime dateTo, int userId, int activityId = 0)
        {
            Logger.Info($"Getting activity logs for user={userId}, period={dateFrom.ToString("yyyy-MM-dd")}-{dateTo.ToString("yyyy-MM-dd")}" +
                $"{(activityId > 0 ? ", activity=" + activityId : "")}");
            using (ISession session = HibernateLoader.CreateSession())
            {
                IQueryable<LifeActivityLog> query;
                if (activityId <= 0)
                {
                    query = from log in session.Query<LifeActivityLog>()
                            where log.UserId == userId && log.Period >= dateFrom && log.Period <= dateTo
                            select log;
                }
                else
                {
                    query = from log in session.Query<LifeActivityLog>()
                            where log.UserId == userId && log.LifeActivityId == activityId && log.Period >= dateFrom && log.Period <= dateTo
                            select log;
                }
                return query.ToArray();
            }
        }

        public static LifeActivityLogValue[] GetLogValuesForPeriod(DateTime dateFrom, DateTime dateTo, int userId)
        {
            Logger.Info($"Getting activity log values for user={userId}, period={dateFrom.ToString("yyyy-MM-dd")}-{dateTo.ToString("yyyy-MM-dd")}");
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from logValue in session.Query<LifeActivityLogValue>()
                            where logValue.UserId == userId && logValue.Period >= dateFrom && logValue.Period <= dateTo
                            select logValue;
                return query.ToArray();
            }
        }

        public static LifeActivityLogWithValues[] GetLogsForDate(
            int activityId, DateTime date, int userId, bool onlyActive = true)
        {
            List<LifeActivityLogWithValues> logList = new List<LifeActivityLogWithValues>();
            using (ISession session = HibernateLoader.CreateSession())
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
            using (ISession session = HibernateLoader.CreateSession())
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
            using (ISession session = HibernateLoader.CreateSession())
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

        public static void UpdateLog(LifeActivityLog log)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                log.UpdateTime = DateTime.Now;
                CommonManager<LifeActivityLog>.Update(log, session);
                session.Flush();
            }
        }

        public static void UpdateLog(LifeActivityLog log, LifeActivityLogValue[] logValues)
        {
            using (ISession session = HibernateLoader.CreateSession())
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
    }
}

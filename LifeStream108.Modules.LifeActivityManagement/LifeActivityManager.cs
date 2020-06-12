using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeActivityManager
    {
        public static (LifeActivity Activity, LifeActivityParameter[] Parameters) GetActivityWithParams(int activityId, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from act in session.Query<LifeActivity>()
                            where act.UserId == userId && act.Id == activityId
                            select act;
                LifeActivity activity = query.FirstOrDefault();
                LifeActivityParameter[] parameters = null;
                if (activity != null)
                {
                    parameters = LifeActivityParameterManager.GetParametersByActivity(activity.Id, session, true);
                }
                return (activity, parameters);
            }
        }

        public static LifeActivity[] GetActivitiesForUser(int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from act in session.Query<LifeActivity>()
                            where act.UserId == userId
                            select act;
                return query.ToArray();
            }
        }

        public static LifeActivity GetActivityByUserCode(int userCode, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                return GetActivityByUserCode(userCode, userId, session);
            }
        }

        public static (LifeActivity Activity, LifeActivityParameter[] Parameters) GetActivityAndParamsByUserCode(int userCode, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                LifeActivity activity = GetActivityByUserCode(userCode, userId, session);
                LifeActivityParameter[] parameters = null;
                if (activity != null)
                {
                    parameters = LifeActivityParameterManager.GetParametersByActivity(activity.Id, session, true);
                }
                return (activity, parameters);
            }
        }

        private static LifeActivity GetActivityByUserCode(int userCode, int userId, DbConnection connection)
        {
            var query = from act in session.Query<LifeActivity>()
                        where act.UserId == userId && act.UserCode == userCode
                        select act;
            return query.FirstOrDefault();
        }

        public static LifeActivity GetActivityByName(string activityName, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from act in session.Query<LifeActivity>()
                            where act.UserId == userId && act.Name.ToUpper() == activityName.ToUpper()
                            select act;
                return query.FirstOrDefault();
            }
        }

        public static LifeActivity[] FindActivitiesByName(string word, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from act in session.Query<LifeActivity>()
                            where act.UserId == userId && act.Name.ToUpper().Contains(word.ToUpper())
                            select act;
                return query.ToArray();
            }
        }

        public static void AddActivity(LifeActivity activity)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                activity.UserCode = GetNextUserCode(activity.UserId, session);
                CommonManager<LifeActivity>.Add(activity, session);
                session.Flush();
            }
        }

        public static void UpdateActivity(LifeActivity activity)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                CommonManager<LifeActivity>.Update(activity, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, DbConnection connection)
        {
            var query = from item in session.Query<LifeActivity>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }

        private static LifeActivity ReadActivity(IDataReader reader)
        {
            LifeActivity act = new LifeActivity();
            act.Id = PgsqlUtils.GetInt("id", reader, 0);
            act.UserCode = PgsqlUtils.GetInt("user_code", reader, 0);
            act.UserId = PgsqlUtils.GetInt("user_id", reader, 0);
            act.Name = PgsqlUtils.GetString("name", reader, "");
            act.ShortName = PgsqlUtils.GetString("short_name", reader, "");
            act.PeriodType = (PeriodicityType)PgsqlUtils.GetEnum("type", reader, typeof(PeriodicityType), PeriodicityType.None);
            act.LifeGroupAtGroupId = PgsqlUtils.GetInt("life_group_at_group_id", reader, 0);
            act.Active = PgsqlUtils.GetBoolean("active", reader, false);
            act.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return act;
        }
    }
}

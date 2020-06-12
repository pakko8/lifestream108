using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeActivityParameterManager
    {
        public static LifeActivityParameter GetParameterByCode(int userCode, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from parameter in session.Query<LifeActivityParameter>()
                            where parameter.UserId == userId && parameter.UserCode == userCode
                            select parameter;
                return query.FirstOrDefault();
            }
        }

        public static LifeActivityParameter GetParameterByName(string paramName, int activityId, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from parameter in session.Query<LifeActivityParameter>()
                            where parameter.UserId == userId && parameter.ActivityId == activityId
                            && parameter.Name.ToUpper() == paramName.ToUpper()
                            select parameter;
                return query.FirstOrDefault();
            }
        }

        public static LifeActivityParameter[] GetParametersForUser(int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from parameter in session.Query<LifeActivityParameter>()
                            where parameter.UserId == userId
                            select parameter;
                return query.ToArray();
            }
        }

        internal static LifeActivityParameter[] GetParametersByActivity(int activityId, DbConnection connection, bool onlyActive = true)
        {
            var query = from parameter in session.Query<LifeActivityParameter>()
                        where parameter.ActivityId == activityId
                        select parameter;
            return onlyActive ? query.Where(n => n.Active).ToArray() : query.ToArray();
        }

        public static void AddParameters(LifeActivityParameter[] parameters)
        {
            int userId = parameters[0].UserId;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                int userCode = GetNextUserCode(userId, session);
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (LifeActivityParameter parameter in parameters)
                        {
                            parameter.UserCode = userCode++;
                            CommonManager<LifeActivityParameter>.Add(parameter, session);
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

        public static void UpdateParameter(LifeActivityParameter parameter)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                CommonManager<LifeActivityParameter>.Update(parameter, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, DbConnection connection)
        {
            var query = from item in session.Query<LifeActivityParameter>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }

        private static LifeActivityParameter ReadParameter(IDataReader reader)
        {
            LifeActivityParameter param = new LifeActivityParameter();
            param.Id = PgsqlUtils.GetInt("id", reader, 0);
            param.SortOrder = PgsqlUtils.GetInt("sort_order", reader, 0);
            param.UserCode = PgsqlUtils.GetInt("user_code", reader, 0);
            param.UserId = PgsqlUtils.GetInt("user_id", reader, 0);
            param.ActivityId = PgsqlUtils.GetInt("activity_id", reader, 0);
            param.Name = PgsqlUtils.GetString("name", reader, "");
            param.MeasureId = PgsqlUtils.GetInt("measure_id", reader, 0);
            param.DataType = (DataType)PgsqlUtils.GetEnum("data_type", reader, typeof(DataType), DataType.Text);
            param.Fuction = PgsqlUtils.GetString("function", reader, "");
            param.Active = PgsqlUtils.GetBoolean("active", reader, false);
            param.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.Now);
            return param;
        }
    }
}

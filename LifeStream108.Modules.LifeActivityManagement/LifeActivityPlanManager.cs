using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public class LifeActivityPlanManager
    {
        public static LifeActivityPlan[] GetPlansForUser(int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                throw new NotImplementedException();
            }
        }

        public static void AddPlan(LifeActivityPlan plan)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                throw new NotImplementedException();
            }
        }

        public static void UpdatePlan(LifeActivityPlan plan)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                throw new NotImplementedException();
            }
        }
    }
}

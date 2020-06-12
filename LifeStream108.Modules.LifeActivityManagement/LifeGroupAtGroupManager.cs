using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System.Data.Common;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeGroupAtGroupManager
    {
        public static LifeGroupAtGroup GetGroupAtGroup(int groupAtGroupId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                return CommonManager<LifeGroupAtGroup>.GetById(groupAtGroupId, session);
            }
        }

        public static LifeGroupAtGroup[] GetGroupsAtGroupsForUser(int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from grp in session.Query<LifeGroupAtGroup>()
                            where grp.UserId == userId
                            select grp;
                return query.ToArray();
            }
        }

        internal static LifeGroupAtGroup GetGroupAtGroup(int groupAtGroupId, int userId, DbConnection connection)
        {
            var query = from assign in session.Query<LifeGroupAtGroup>()
                        where assign.UserId == userId && assign.Id == groupAtGroupId
                        select assign;
            return query.FirstOrDefault();
        }

        public static LifeGroupAtGroup GetGroupAtGroupByGroups(int groupId, int parentGroupId, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from assign in session.Query<LifeGroupAtGroup>()
                            where assign.UserId == userId && assign.LifeGroupId == groupId && assign.ParentLifeGroupId == parentGroupId
                            select assign;
                return query.FirstOrDefault();
            }
        }

        public static LifeGroupAtGroup[] GetGroupAtGroupsByGroup(int groupId, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from assign in session.Query<LifeGroupAtGroup>()
                            where assign.UserId == userId && assign.LifeGroupId == groupId
                            select assign;
                return query.ToArray();
            }
        }

        public static void AddGroupAtGroup(LifeGroupAtGroup item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                CommonManager<LifeGroupAtGroup>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateGroupAtGroup(LifeGroupAtGroup item)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                CommonManager<LifeGroupAtGroup>.Update(item, session);
                session.Flush();
            }
        }
    }
}

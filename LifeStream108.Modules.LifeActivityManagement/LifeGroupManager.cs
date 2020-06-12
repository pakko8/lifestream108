using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Data.Common;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeGroupManager
    {
        public static LifeGroup GetGroup(int groupId, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                return GetGroup(groupId, userId, session);
            }
        }

        private static LifeGroup GetGroup(int groupId, int userId, DbConnection connection)
        {
            var query = from grp in session.Query<LifeGroup>()
                        where grp.UserId == userId && grp.Id == groupId
                        select grp;
            return query.FirstOrDefault();
        }

        public static LifeGroup GetGroupByCode(int userCode, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from grp in session.Query<LifeGroup>()
                            where grp.UserId == userId && grp.UserCode == userCode
                            select grp;
                return query.FirstOrDefault();
            }
        }

        public static LifeGroup GetGroupByGroupAtGroup(int groupAtGroupId, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                LifeGroupAtGroup groupAtGroup = LifeGroupAtGroupManager.GetGroupAtGroup(groupAtGroupId, userId, session);
                return GetGroup(groupAtGroup.LifeGroupId, userId, session);
            }
        }

        public static LifeGroup[] GetGroupsForUser(int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from grp in session.Query<LifeGroup>()
                            where grp.UserId == userId
                            select grp;
                return query.ToArray();
            }
        }

        public static LifeGroup GetGroupByName(string groupName, int userId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var query = from grp in session.Query<LifeGroup>()
                            where grp.UserId == userId && grp.Name.ToUpper() == groupName.ToUpper()
                            select grp;
                return query.FirstOrDefault();
            }
        }

        public static void AddGroup(LifeGroup group)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        group.UserCode = GetNextUserCode(group.UserId, session);
                        CommonManager<LifeGroup>.Add(group, session);
                        LifeGroupAtGroup groupAtGroup = new LifeGroupAtGroup
                        {
                            UserId = group.UserId,
                            LifeGroupId = group.Id,
                            ParentLifeGroupId = 0
                        };
                        CommonManager<LifeGroupAtGroup>.Add(groupAtGroup, session);
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

        public static void UpdateGroup(LifeGroup group)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                CommonManager<LifeGroup>.Update(group, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, DbConnection connection)
        {
            var query = from item in session.Query<LifeGroup>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }
    }
}

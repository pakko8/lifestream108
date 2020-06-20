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
    public static class LifeGroupAtGroupManager
    {
        internal const string TableName = "activities.life_groups_at_groups";

        public static LifeGroupAtGroup GetGroupAtGroup(int groupAtGroupId)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where id={groupAtGroupId}", ReadGroupAtGroup);
        }

        public static LifeGroupAtGroup[] GetGroupsAtGroupsForUser(int userId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where user_id={userId}", ReadGroupAtGroup);
        }

        public static LifeGroupAtGroup GetGroupAtGroupByGroups(int groupId, int parentGroupId, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where use_id={userId} and life_group_id={groupId} and parent_life_group_id={parentGroupId}",
                ReadGroupAtGroup);
        }

        public static LifeGroupAtGroup[] GetGroupAtGroupsByGroup(int groupId, int userId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where user_id={userId} and life_group_id={groupId}",
                ReadGroupAtGroup);
        }

        public static void AddGroupAtGroup(LifeGroupAtGroup grpAtGrp)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();
                AddGroupAtGroup(grpAtGrp, connection);
            }
        }

        internal static void AddGroupAtGroup(LifeGroupAtGroup grpAtGrp, NpgsqlConnection connection)
        {
            string query =
                $@"insert into {TableName}
                (
                    user_id,
                    life_group_id,
                    parent_life_group_id,
                    sort_order,
                    reg_time
                )
                values
                (
                    @user_id,
                    @life_group_id,
                    @parent_life_group_id,
                    @sort_order,
                    current_timestamp
                )
                returning id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@user_id", grpAtGrp.UserId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@life_group_id", grpAtGrp.LifeGroupId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@parent_life_group_id", grpAtGrp.ParentLifeGroupId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@sort_order", grpAtGrp.SortOrder, NpgsqlDbType.Integer),
            };

            grpAtGrp.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);
        }

        public static void UpdateGroupAtGroup(LifeGroupAtGroup grpAtGrp)
        {
            string query =
                $@"update {TableName}
                set
                    life_group_id=@life_group_id,
                    parent_life_group_id=@parent_life_group_id,
                    sort_order=@sort_order,
                where id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", grpAtGrp.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@life_group_id", grpAtGrp.LifeGroupId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@parent_life_group_id", grpAtGrp.ParentLifeGroupId, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@sort_order", grpAtGrp.SortOrder, NpgsqlDbType.Integer),
            };

            PostgreSqlCommandUtils.UpdateEntity(query, parameters);
        }

        private static LifeGroupAtGroup ReadGroupAtGroup(IDataReader reader)
        {
            LifeGroupAtGroup grpAtGrp = new LifeGroupAtGroup();
            grpAtGrp.Id = PgsqlUtils.GetInt("id", reader);
            grpAtGrp.UserId = PgsqlUtils.GetInt("user_id", reader);
            grpAtGrp.LifeGroupId = PgsqlUtils.GetInt("life_group_id", reader);
            grpAtGrp.ParentLifeGroupId = PgsqlUtils.GetInt("parent_life_group_id", reader);
            grpAtGrp.SortOrder = PgsqlUtils.GetInt("sort_order", reader);
            grpAtGrp.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return grpAtGrp;
        }
    }
}

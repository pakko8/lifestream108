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
    public static class LifeGroupManager
    {
        private const string TableName = "activities.life_groups";

        public static LifeGroup GetGroup(int groupId, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where user_id={userId} and id={groupId}", ReadGroup);
        }

        public static LifeGroup GetGroupByCode(int userCode, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and user_code={userCode}", ReadGroup);
        }

        public static LifeGroup GetGroupByGroupAtGroup(int groupAtGroupId, int userId)
        {
            string query = $@"select grp.* from {TableName} grp
                inner join {LifeGroupAtGroupManager.TableName} grpatgrp on grpatgrp.life_group_id=grp.id
                where grpatgrp.user_id={userId} and grpatgrp.id={groupAtGroupId}";
            return PostgreSqlCommandUtils.GetEntity(query, ReadGroup);
        }

        public static LifeGroup[] GetGroupsForUser(int userId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where user_id={userId}", ReadGroup);
        }

        public static LifeGroup GetGroupByName(string groupName, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and upper(name)='{groupName.ToUpper()}'", ReadGroup);
        }

        public static void AddGroup(LifeGroup group)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();

                group.UserCode = GetNextUserCode(group.UserId, connection);

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query =
                            $@"insert into {TableName}
                            (
                                user_id,
                                user_code,
                                name,
                                short_name,
                                type,
                                active,
                                reg_time
                            )
                            values
                            (
                                @user_id,
                                @user_code,
                                @name,
                                @short_name,
                                @type,
                                @active,
                                current_timestamp
                            )
                            returning id";

                        NpgsqlParameter[] parameters = new NpgsqlParameter[]
                        {
                            PostgreSqlCommandUtils.CreateParam("@user_id", group.UserId, NpgsqlDbType.Integer),
                            PostgreSqlCommandUtils.CreateParam("@user_code", group.UserCode, NpgsqlDbType.Integer),
                            PostgreSqlCommandUtils.CreateParam("@name", group.Name, NpgsqlDbType.Varchar),
                            PostgreSqlCommandUtils.CreateParam("@short_name", group.ShortName, NpgsqlDbType.Varchar),
                            PostgreSqlCommandUtils.CreateParam("@type", (int)group.FinanceType, NpgsqlDbType.Integer),
                            PostgreSqlCommandUtils.CreateParam("@active", group.Active, NpgsqlDbType.Boolean),
                        };

                        group.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);

                        LifeGroupAtGroup groupAtGroup = new LifeGroupAtGroup
                        {
                            UserId = group.UserId,
                            LifeGroupId = group.Id,
                            ParentLifeGroupId = 0
                        };
                        LifeGroupAtGroupManager.AddGroupAtGroup(groupAtGroup, connection);

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
            string query =
                $@"update {TableName}
                set
                    user_code=@user_code,
                    name=@name,
                    short_name=@short_name,
                    active=@active,
                    type=@type
                where
                    id=@id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                PostgreSqlCommandUtils.CreateParam("@id", group.Id, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@user_code", group.UserCode, NpgsqlDbType.Integer),
                PostgreSqlCommandUtils.CreateParam("@name", group.Name, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@short_name", group.ShortName, NpgsqlDbType.Varchar),
                PostgreSqlCommandUtils.CreateParam("@active", group.Active, NpgsqlDbType.Boolean),
                PostgreSqlCommandUtils.CreateParam("@type", (int)group.FinanceType, NpgsqlDbType.Integer)
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

        private static LifeGroup ReadGroup(IDataReader reader)
        {
            LifeGroup group = new LifeGroup();
            group.Id = PgsqlUtils.GetInt("id", reader);
            group.UserCode = PgsqlUtils.GetInt("user_code", reader);
            group.UserId = PgsqlUtils.GetInt("user_id", reader);
            group.Name = PgsqlUtils.GetString("name", reader);
            group.ShortName = PgsqlUtils.GetString("short_name", reader);
            group.Active = PgsqlUtils.GetBoolean("active", reader);
            group.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            group.FinanceType = (FinancialType)PgsqlUtils.GetEnum("type", reader, typeof(FinancialType), FinancialType.None);
            return group;
        }
    }
}

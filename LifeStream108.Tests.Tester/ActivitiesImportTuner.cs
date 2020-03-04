using LifeStream108.Libs.Entities;
using LifeStream108.Libs.HibernateManagement;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using NHibernate;
using Npgsql;
using System;
using System.Linq;

namespace LifeStream108.Tests.Tester
{
    /// <summary>
    /// Move binding from "parent_life_group_id" to "life_group_at_group_id" in table "activities.life_activities"
    /// </summary>
    internal static class ActivitiesImportTuner
    {
        private const int UserId = 1;

        public static void Run()
        {
            LifeActivity[] activities = LifeActivityManager.GetActivitiesForUser(UserId);
            LifeGroupAtGroup[] groupAtGroups = LifeGroupAtGroupManager.GetGroupsAtGroupsForUser(UserId);
            foreach (LifeActivity activity in activities)
            {
                int parentGroupId = GetParentLifeGroup(activity.Id);
                LifeGroupAtGroup groupAtGroup = groupAtGroups.FirstOrDefault(n => n.LifeGroupId == parentGroupId);
                if (groupAtGroup == null) continue;
                activity.LifeGroupAtGroupId = groupAtGroup.Id;
                LifeActivityManager.UpdateActivity(activity);
            }
        }

        private static int GetParentLifeGroup(int activityId)
        {
            int groupId = -1;
            using (ISession session = HibernateLoader.CreateSession())
            {
                string commandText = "select parent_life_group_id from activities.life_activities where id=" + activityId;
                NpgsqlCommand command = new NpgsqlCommand(commandText, (NpgsqlConnection)session.Connection);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        groupId = Convert.ToInt32(reader["parent_life_group_id"]);
                    }
                }
            }
            return groupId;
        }
    }
}

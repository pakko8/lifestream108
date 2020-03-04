using LifeStream108.Libs.Entities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement.Managers
{
    public static class LifeGroupAtGroupManager
    {
        public static LifeGroupAtGroup GetGroupAtGroup(int groupAtGroupId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return CommonManager<LifeGroupAtGroup>.GetById(groupAtGroupId, session);
            }
        }

        public static LifeGroupAtGroup[] GetGroupsAtGroupsForUser(int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from grp in session.Query<LifeGroupAtGroup>()
                            where grp.UserId == userId
                            select grp;
                return query.ToArray();
            }
        }

        internal static LifeGroupAtGroup GetGroupAtGroup(int groupAtGroupId, int userId, ISession session)
        {
            var query = from assign in session.Query<LifeGroupAtGroup>()
                        where assign.UserId == userId && assign.Id == groupAtGroupId
                        select assign;
            return query.FirstOrDefault();
        }

        public static LifeGroupAtGroup GetGroupAtGroupByGroups(int groupId, int parentGroupId, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from assign in session.Query<LifeGroupAtGroup>()
                            where assign.UserId == userId && assign.LifeGroupId == groupId && assign.ParentLifeGroupId == parentGroupId
                            select assign;
                return query.FirstOrDefault();
            }
        }

        public static LifeGroupAtGroup[] GetGroupAtGroupsByGroup(int groupId, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from assign in session.Query<LifeGroupAtGroup>()
                            where assign.UserId == userId && assign.LifeGroupId == groupId
                            select assign;
                return query.ToArray();
            }
        }
        public static void AddGroupAtGroup(LifeGroupAtGroup item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<LifeGroupAtGroup>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateGroupAtGroup(LifeGroupAtGroup item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<LifeGroupAtGroup>.Update(item, session);
                session.Flush();
            }
        }
    }
}

using LifeStream108.Libs.Entities.NewsEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.NewsManagement.Managers
{
    public static class NewsGroupManager
    {
        public static NewsGroup[] GetAllActiveGroups()
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from grp in session.Query<NewsGroup>()
                            where grp.Active
                            select grp;
                return query.ToArray();
            }
        }

        public static void UpdateGroup(NewsGroup group)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<NewsGroup>.Update(group, session);
                session.Flush();
            }
        }
    }
}

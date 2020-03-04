using LifeStream108.Libs.Entities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.NewsManagement.Managers
{
    public static class NewsItemManager
    {
        public static NewsItem GetNewsItemByResourceId(string resourceId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from item in session.Query<NewsItem>()
                            where item.ResourceId == resourceId
                            select item;
                return query.FirstOrDefault();
            }
        }

        public static NewsItem GetLastNewsItem(int groupId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from item in session.Query<NewsItem>()
                            where item.NewsGroupId == groupId
                            orderby item.NewsTime descending
                            select item;
                return query.FirstOrDefault();
            }
        }

        public static void AddNewsItem(NewsItem item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<NewsItem>.Add(item, session);
                session.Flush();
            }
        }
    }
}

using LifeStream108.Libs.Entities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.TempDataManagement.Managers
{
    public static class SessionManager
    {
        public static Session[] GetAllSessions()
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return CommonManager<Session>.GetAll(session);
            }
        }

        public static Session GetSessionForUser(int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from item in session.Query<Session>()
                            where item.UserId == userId
                            select item;
                return query.FirstOrDefault();
            }
        }

        public static void AddSession(Session item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<Session>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateSession(Session item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<Session>.Update(item, session);
                session.Flush();
            }
        }

        public static void Delete(Session item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<Session>.Delete(item, session);
                session.Flush();
            }
        }
    }
}

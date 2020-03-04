using NHibernate;
using System.Linq;

namespace LifeStream108.Libs.HibernateManagement
{
    public class CommonManager<T> where T : class
    {
        public static void Add(T item, ISession session)
        {
            session.Save(item);
        }

        public static void Update(T item, ISession session)
        {
            session.Update(item);
        }

        public static void Delete(T item, ISession session)
        {
            session.Delete(item);
        }

        public static T GetById(object id, ISession session)
        {
            return session.Get<T>(id);
        }

        public static T[] GetAll(ISession session)
        {
            var query = from t in session.Query<T>()
                        select t;
            return query.ToArray();
        }
    }
}

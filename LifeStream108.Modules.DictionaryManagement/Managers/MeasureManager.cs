using LifeStream108.Libs.Entities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.DictionaryManagement.Managers
{
    public static class MeasureManager
    {
        public static Measure GetMeasureByName(string measureName, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from measure in session.Query<Measure>()
                            where measure.UserId == userId && measure.Name.ToUpper() == measureName.ToUpper()
                            select measure;
                return query.FirstOrDefault();
            }
        }

        public static Measure[] GetMeasuresForUser(int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from measure in session.Query<Measure>()
                            where measure.UserId == userId
                            select measure;
                return query.ToArray();
            }
        }

        public static void AddMeasure(Measure measure)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                measure.UserCode = GetNextUserCode(measure.UserId, session);
                CommonManager<Measure>.Add(measure, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, ISession session)
        {
            var query = from item in session.Query<Measure>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }
    }
}

using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.ToDoListManagement
{
    public static class ToDoCategoryManager
    {
        public static ToDoCategory[] GetUserCategories(int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from cat in session.Query<ToDoCategory>()
                            where cat.UserId == userId
                            select cat;
                return query.ToArray();
            }
        }

        public static ToDoCategory GetCategoryByCode(int categoryCode, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from cat in session.Query<ToDoCategory>()
                            where cat.UserCode == categoryCode && cat.UserId == userId
                            select cat;
                return query.FirstOrDefault();
            }
        }

        public static void AddCategory(ToDoCategory item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                item.UserCode = GetNextUserCode(item.UserId, session);
                CommonManager<ToDoCategory>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateCategory(ToDoCategory item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<ToDoCategory>.Update(item, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, ISession session)
        {
            var query = from item in session.Query<ToDoCategory>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }
    }
}

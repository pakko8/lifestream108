using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.ToDoListManagement.Managers
{
    public static class ToDoListManager
    {
        public static ToDoList[] GetUserLists(int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from list in session.Query<ToDoList>()
                            where list.UserId == userId
                            select list;
                return query.ToArray();
            }
        }

        public static ToDoList[] GetCategoryLists(int categoryId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from list in session.Query<ToDoList>()
                            where list.CategoryId == categoryId
                            select list;
                return query.ToArray();
            }
        }

        public static ToDoList GetListByCode(int code, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from list in session.Query<ToDoList>()
                            where list.UserCode == code && list.UserId == userId
                            select list;
                return query.FirstOrDefault();
            }
        }

        public static void AddList(ToDoList item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                item.UserCode = GetNextUserCode(item.UserId, session);
                CommonManager<ToDoList>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateList(ToDoList item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<ToDoList>.Update(item, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, ISession session)
        {
            var query = from item in session.Query<ToDoList>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }
    }
}

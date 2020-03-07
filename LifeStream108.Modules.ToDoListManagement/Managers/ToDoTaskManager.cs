using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.ToDoListManagement.Managers
{
    public static class ToDoTaskManager
    {
        public static void AddTask(ToDoTask item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                item.UserCode = GetNextUserCode(item.UserId, session);
                CommonManager<ToDoTask>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateTask(ToDoTask item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<ToDoTask>.Update(item, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, ISession session)
        {
            var query = from item in session.Query<ToDoTask>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }
    }
}

using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;

namespace LifeStream108.Modules.ToDoListManagement.Managers
{
    public static class ToDoReminderHistoryManager
    {
        public static void AddCategory(ToDoReminderHistory item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<ToDoReminderHistory>.Add(item, session);
                session.Flush();
            }
        }
    }
}

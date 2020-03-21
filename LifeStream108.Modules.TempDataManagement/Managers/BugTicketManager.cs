using LifeStream108.Libs.Entities.TicketEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;

namespace LifeStream108.Modules.TempDataManagement.Managers
{
    public static class BugTicketManager
    {
        public static void AddBugTicket(BugTicket ticket)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<BugTicket>.Add(ticket, session);
                session.Flush();
            }
        }
    }
}

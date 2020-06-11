using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public class LifeActivityPlanManager
    {
        public static LifeActivityPlan[] GetPlansForUser(int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from plan in session.Query<LifeActivityPlan>()
                            where plan.UserId == userId
                            select plan;
                return query.ToArray();
            }
        }

        public static void AddPlan(LifeActivityPlan plan)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<LifeActivityPlan>.Add(plan, session);
                session.Flush();
            }
        }

        public static void UpdatePlan(LifeActivityPlan plan)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<LifeActivityPlan>.Update(plan, session);
                session.Flush();
            }
        }
    }
}

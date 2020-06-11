using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeActivityParameterManager
    {
        public static LifeActivityParameter GetParameterByCode(int userCode, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from parameter in session.Query<LifeActivityParameter>()
                            where parameter.UserId == userId && parameter.UserCode == userCode
                            select parameter;
                return query.FirstOrDefault();
            }
        }

        public static LifeActivityParameter GetParameterByName(string paramName, int activityId, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from parameter in session.Query<LifeActivityParameter>()
                            where parameter.UserId == userId && parameter.ActivityId == activityId
                            && parameter.Name.ToUpper() == paramName.ToUpper()
                            select parameter;
                return query.FirstOrDefault();
            }
        }

        public static LifeActivityParameter[] GetParametersForUser(int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from parameter in session.Query<LifeActivityParameter>()
                            where parameter.UserId == userId
                            select parameter;
                return query.ToArray();
            }
        }

        internal static LifeActivityParameter[] GetParametersByActivity(int activityId, ISession session, bool onlyActive = true)
        {
            var query = from parameter in session.Query<LifeActivityParameter>()
                        where parameter.ActivityId == activityId
                        select parameter;
            return onlyActive ? query.Where(n => n.Active).ToArray() : query.ToArray();
        }

        public static void AddParameters(LifeActivityParameter[] parameters)
        {
            int userId = parameters[0].UserId;
            using (ISession session = HibernateLoader.CreateSession())
            {
                int userCode = GetNextUserCode(userId, session);
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (LifeActivityParameter parameter in parameters)
                        {
                            parameter.UserCode = userCode++;
                            CommonManager<LifeActivityParameter>.Add(parameter, session);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public static void UpdateParameter(LifeActivityParameter parameter)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<LifeActivityParameter>.Update(parameter, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, ISession session)
        {
            var query = from item in session.Query<LifeActivityParameter>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }
    }
}

using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System;
using System.Linq;

namespace LifeStream108.Modules.LifeActivityManagement
{
    public static class LifeActivityManager
    {
        public static (LifeActivity Activity, LifeActivityParameter[] Parameters) GetActivityWithParams(int activityId, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from act in session.Query<LifeActivity>()
                            where act.UserId == userId && act.Id == activityId
                            select act;
                LifeActivity activity = query.FirstOrDefault();
                LifeActivityParameter[] parameters = null;
                if (activity != null)
                {
                    parameters = LifeActivityParameterManager.GetParametersByActivity(activity.Id, session, true);
                }
                return (activity, parameters);
            }
        }

        public static LifeActivity[] GetActivitiesForUser(int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from act in session.Query<LifeActivity>()
                            where act.UserId == userId
                            select act;
                return query.ToArray();
            }
        }

        public static LifeActivity GetActivityByUserCode(int userCode, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return GetActivityByUserCode(userCode, userId, session);
            }
        }

        public static (LifeActivity Activity, LifeActivityParameter[] Parameters) GetActivityAndParamsByUserCode(int userCode, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                LifeActivity activity = GetActivityByUserCode(userCode, userId, session);
                LifeActivityParameter[] parameters = null;
                if (activity != null)
                {
                    parameters = LifeActivityParameterManager.GetParametersByActivity(activity.Id, session, true);
                }
                return (activity, parameters);
            }
        }

        private static LifeActivity GetActivityByUserCode(int userCode, int userId, ISession session)
        {
            var query = from act in session.Query<LifeActivity>()
                        where act.UserId == userId && act.UserCode == userCode
                        select act;
            return query.FirstOrDefault();
        }

        public static LifeActivity GetActivityByName(string activityName, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from act in session.Query<LifeActivity>()
                            where act.UserId == userId && act.Name.ToUpper() == activityName.ToUpper()
                            select act;
                return query.FirstOrDefault();
            }
        }

        public static LifeActivity[] FindActivitiesByName(string word, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from act in session.Query<LifeActivity>()
                            where act.UserId == userId && act.Name.ToUpper().Contains(word.ToUpper())
                            select act;
                return query.ToArray();
            }
        }

        public static void AddActivity(LifeActivity activity)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                activity.UserCode = GetNextUserCode(activity.UserId, session);
                CommonManager<LifeActivity>.Add(activity, session);
                session.Flush();
            }
        }

        public static void UpdateActivity(LifeActivity activity)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<LifeActivity>.Update(activity, session);
                session.Flush();
            }
        }

        private static int GetNextUserCode(int userId, ISession session)
        {
            var query = from item in session.Query<LifeActivity>()
                        where item.UserId == userId
                        orderby item.UserCode descending
                        select item.UserCode;
            return query.FirstOrDefault() + 1;
        }
    }
}

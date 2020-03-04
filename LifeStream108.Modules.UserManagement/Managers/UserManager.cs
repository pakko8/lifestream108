using LifeStream108.Libs.Entities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System;
using System.Linq;

namespace LifeStream108.Modules.UserManagement.Managers
{
    public static class UserManager
    {
        public static User[] GetAllUsers()
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return CommonManager<User>.GetAll(session);
            }
        }

        public static Tuple<bool, string, User> AuthorizeUser(int telegramId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                User user = GetUserByTelegramId(telegramId, session);
                if (user == null)
                    return new Tuple<bool, string, User>(false, "Пользователь не зарегистрирован", null);
                if (user.Status != UserStatus.Active)
                    return new Tuple<bool, string, User>(false, "Пользователь " + user.Status.GetDescriptiveString(), null);

                return new Tuple<bool, string, User>(true, "", user);
            }
        }

        public static User GetUserByTelegramId(int telegramId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return GetUserByTelegramId(telegramId, session);
            }
        }

        public static User GetSuperuser()
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from user in session.Query<User>()
                            where user.Superuser
                            select user;
                return query.FirstOrDefault();
            }
        }

        private static User GetUserByTelegramId(int telegramId, ISession session)
        {
            var query = from user in session.Query<User>()
                        where user.TelegramId == telegramId
                        select user;
            return query.FirstOrDefault();
        }
    }
}

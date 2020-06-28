using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.UserEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using System;
using System.Data;

namespace LifeStream108.Modules.UserManagement
{
    public static class UserManager
    {
        private const string TableName = "users.users";

        public static User AuthorizeUser(string email, string passwordHash)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where upper(email)='{email.ToUpper()}' and password_hash='{passwordHash}'", ReadUser);
        }

        public static (User User, string Error) AuthorizeUser(int telegramId)
        {
            User user = GetUserByTelegramId(telegramId);
            if (user == null)
                return (null, "Пользователь не зарегистрирован");
            if (user.Status != UserStatus.Active)
                return (null, "Пользователь " + user.Status.GetDescriptiveString());

            return (user, "");

        }

        public static User GetUser(int userId)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where id={userId}", ReadUser);
        }

        public static User[] GetAllUsers()
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where superuser='t'", ReadUser);
        }

        public static User GetUserByTelegramId(int telegramId)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where telegram_id={telegramId}", ReadUser);
        }

        public static User GetSuperuser()
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where superuser='t'", ReadUser);
        }

        private static User ReadUser(IDataReader reader)
        {
            User user = new User();
            user.Id = PgsqlUtils.GetInt("id", reader);
            user.Email = PgsqlUtils.GetString("email", reader);
            user.Name = PgsqlUtils.GetString("name", reader);
            user.Superuser = PgsqlUtils.GetBoolean("superuser", reader);
            user.TelegramId = PgsqlUtils.GetInt("telegram_id", reader);
            user.LanguageId = PgsqlUtils.GetInt("language_id", reader);
            user.CurrencyId = PgsqlUtils.GetInt("currency_id", reader);
            user.DefaultProjectId = PgsqlUtils.GetInt("default_project_id", reader);
            //user.CheckActLogsTime = PgsqlUtils.GetDateTime("check_act_logs_last_time", reader, DateTime.MinValue);
            user.Status = (UserStatus)PgsqlUtils.GetEnum("status", reader, typeof(UserStatus), UserStatus.Bloked);
            return user;
        }
    }
}

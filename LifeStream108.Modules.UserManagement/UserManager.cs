using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.UserEntities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace LifeStream108.Modules.UserManagement
{
    public static class UserManager
    {
        private const string TableName = "users.users";

        public static User AuthorizeUser(string email, string passwordHash, string dbConnString)
        {
            User user = null;
            using (var connection = new NpgsqlConnection(dbConnString))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName} where email='{email.ToUpper()}' and password_hash='{passwordHash}'";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = ReadUser(reader);
                    }
                }
            }
            return user;
        }

        public static (User User, string Error) AuthorizeUser(int telegramId, string dbConnString)
        {
            using (var connection = new NpgsqlConnection(dbConnString))
            {
                connection.Open();
                User user = GetUserByTelegramId(telegramId, connection);
                if (user == null)
                    return (null, "Пользователь не зарегистрирован");
                if (user.Status != UserStatus.Active)
                    return (null, "Пользователь " + user.Status.GetDescriptiveString());

                return (user, "");
            }
        }

        public static User GetUser(int userId, string dbConnString)
        {
            User user = null;
            using (var connection = new NpgsqlConnection(dbConnString))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName} where id={userId}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = ReadUser(reader);
                    }
                }
            }
            return user;
        }

        public static User[] GetAllUsers(string dbConnString)
        {
            List<User> users = new List<User>();
            using (var connection = new NpgsqlConnection(dbConnString))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName} where superuser='t'";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        users.Add(ReadUser(reader));
                    }
                }
            }
            return users.ToArray();
        }

        public static User GetUserByTelegramId(int telegramId, string dbConnString)
        {
            using (var connection = new NpgsqlConnection(dbConnString))
            {
                connection.Open();
                return GetUserByTelegramId(telegramId, connection);
            }
        }

        public static User GetSuperuser(string dbConnString)
        {
            User user = null;
            using (var connection = new NpgsqlConnection(dbConnString))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from {TableName} where superuser='t'";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = ReadUser(reader);
                    }
                }
            }
            return user;
        }

        private static User GetUserByTelegramId(int telegramId, IDbConnection connection)
        {
            User user = null;
            var command = connection.CreateCommand();
            command.CommandText = $"select * from {TableName} where telegram_id={telegramId}";
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = ReadUser(reader);
                }
            }
            return user;
        }

        private static User ReadUser(IDataReader reader)
        {
            User user = new User();
            user.Id = PgsqlUtils.GetInt("id", reader, 0);
            user.Email = PgsqlUtils.GetString("email", reader, "");
            user.Name = PgsqlUtils.GetString("name", reader, "");
            user.Superuser = PgsqlUtils.GetBoolean("superuser", reader, false);
            user.TelegramId = PgsqlUtils.GetInt("telegram_id", reader, 0);
            user.LanguageId = PgsqlUtils.GetInt("language_id", reader, 0);
            user.CurrencyId = PgsqlUtils.GetInt("currency_id", reader, 0);
            user.DefaultProjectId = PgsqlUtils.GetInt("default_project_id", reader, 0);
            user.CheckActLogsTime = PgsqlUtils.GetDateTime("check_act_logs_last_time", reader, DateTime.MinValue);
            user.Status = (UserStatus)PgsqlUtils.GetEnum("status", reader, typeof(UserStatus), UserStatus.Bloked);
            return user;
        }
    }
}

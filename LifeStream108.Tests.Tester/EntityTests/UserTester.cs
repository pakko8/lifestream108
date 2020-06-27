using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.UserEntities;
using LifeStream108.Modules.UserManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class UserTester
    {
        public const int UserId_1 = 1;
        public const int UserId_2 = 2;

        public static void Run()
        {
            User authUserByEmail = UserManager.AuthorizeUser("alexx.silver@gmail.com", CryptoUtils.GenerateSha256Hash("Strong_123"));
            Assert.IsNotNull(authUserByEmail, "Auth user by email is failure");

            User authUserByTelegram = UserManager.GetUserByTelegramId(302115880);
            Assert.IsNotNull(authUserByTelegram, "Auth user by Telegram is failure");

            User userById = UserManager.GetUser(authUserByTelegram.Id);
            Assert.IsNotNull(userById, "Get user by id not work");
        }
    }
}

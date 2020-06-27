using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.TempDataManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class SessionTester
    {
        public static void Run()
        {
            DeleteSessions();

            Session session = new Session
            {
                UserId = UserTester.UserId_1
            };
            SessionManager.AddSession(session);

            Session sessionForUser = SessionManager.GetSessionForUser(UserTester.UserId_1);
            Assert.NotNull(sessionForUser, "Get session for user is failure");

            sessionForUser.LastCommandId = 10;
            sessionForUser.LastRequestText = "Request";
            SessionManager.UpdateSession(sessionForUser);
            sessionForUser = SessionManager.GetSessionForUser(UserTester.UserId_1);
            Assert.IsTrue(sessionForUser.LastCommandId == 10 && sessionForUser.LastRequestText == "Request", "Session not updated");

            SessionManager.Delete(sessionForUser);
            sessionForUser = SessionManager.GetSessionForUser(UserTester.UserId_1);
            Assert.IsNull(sessionForUser, "Delete session is failure");
        }

        private static void DeleteSessions()
        {
            string tableName = TestHelper.GetClassConstantValue("TableName", typeof(SessionManager));
            PostgreSqlCommandUtils.UpdateEntity($"delete from {tableName}", null);
        }
    }
}

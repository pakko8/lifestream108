using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.UserEntities;
using LifeStream108.Modules.TempDataManagement.Managers;
using LifeStream108.Modules.UserManagement.Managers;
using NLog;
using System;
using System.Linq;

namespace LifeStream108.Services.TelegramBotService
{
    internal class ClearSessionsProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private User[] _allUsers = null;

        public void Run(int expireInMinutes)
        {
            Session[] allSessions = SessionManager.GetAllSessions();
            foreach (Session session in allSessions)
            {
                if ((DateTime.Now - session.LastActivityTime).TotalMinutes >= expireInMinutes)
                {
                    if (allSessions.Length > 0) _allUsers = UserManager.GetAllUsers();

                    SessionManager.Delete(session);
                    User user = _allUsers.FirstOrDefault(n => n.Id == session.UserId);
                    Logger.Info("Deleted session for user " + user);
                }
            }
        }
    }
}

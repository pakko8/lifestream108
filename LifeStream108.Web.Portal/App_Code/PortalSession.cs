using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities;
using LifeStream108.Modules.UserManagement.Managers;
using System;
using System.Web;
using System.Web.SessionState;

namespace LifeStream108.Web.Portal.App_Code
{
    public class PortalSession
    {
        public static bool IsAuthorized => User != null;

        public static bool AuthorizeUser(string email, string password)
        {
            string passwordHash = CryptoUtils.GenerateSha256Hash(password);
            User user = UserManager.AuthorizeUser(email, passwordHash);
            if (user == null)
            {
                LastErrorMessage = "По введенным логину и паролю пользователь не найден";
                return false;
            }

            SaveSessionValue(user, "User");
            return true;
        }

        public static int SelectedCategoryId
        {
            get { return GetSessionIntValue("SelectedCategoryId", 0); }
            set { SaveSessionValue(value, "SelectedCategoryId"); }
        }

        public static User User
        {
            get
            {
                object sessionObject = GetSessionObjectValue("User");
                if (sessionObject == null) return null;

                return (User)sessionObject;
            }
        }

        public static string LastErrorMessage
        {
            get
            {
                return GetSessionStringValue("LastErrorMessage", "");
            }
            set
            {
                HttpSessionState session = HttpContext.Current.Session;
                if (session == null) return;

                session["LastErrorMessage"] = value;
            }
        }

        private static string GetSessionStringValue(string keyName, string defaultValue)
        {
            object sessionObject = GetSessionObjectValue(keyName);
            if (sessionObject == null) return defaultValue;

            return sessionObject.ToString();
        }

        private static int GetSessionIntValue(string keyName, int defaultValue)
        {
            return Convert.ToInt32(GetSessionStringValue(keyName, defaultValue.ToString()));
        }

        public static object GetSessionObjectValue(string keyName)
        {
            HttpSessionState session = HttpContext.Current.Session;
            if (session == null) return null;

            return session[keyName];
        }

        private static void SaveSessionValue(object obj, string keyName)
        {
            HttpContext.Current.Session[keyName] = obj;
        }
    }
}
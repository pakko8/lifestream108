using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.UserManagement.Managers;
using System;
using System.Web;
using System.Web.SessionState;

namespace LifeStream108.Web.Portal.App_Code
{
    public class PortalSession
    {
        public static bool IsAuthorized => User != null;

        public static string AuthorizeUser(string email, string password)
        {
            string passwordHash = CryptoUtils.GenerateSha256Hash(password);
            User user = UserManager.AuthorizeUser(email, passwordHash);
            if (user == null)
            {
                return "По введенным логину и паролю пользователь не найден";
            }

            SaveSessionValue(user, "User");
            return null;
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

        public static ToDoCategory[] ToDoCategories
        {
            get
            {
                object sessionObject = GetSessionObjectValue("ToDoCategories");
                if (sessionObject == null) return null;

                return (ToDoCategory[])sessionObject;
            }
            set { SaveSessionValue(value, "ToDoCategories"); }
        }

        public static ToDoList[] ToDoLists
        {
            get
            {
                object sessionObject = GetSessionObjectValue("ToDoLists");
                if (sessionObject == null) return null;

                return (ToDoList[])sessionObject;
            }
            set { SaveSessionValue(value, "ToDoLists"); }
        }

        public static ToDoTask[] ToDoTasks
        {
            get
            {
                object sessionObject = GetSessionObjectValue("ToDoTasks");
                if (sessionObject == null) return null;

                return (ToDoTask[])sessionObject;
            }
            set { SaveSessionValue(value, "ToDoTasks"); }
        }

        public static int SelectedCategoryId
        {
            get { return GetSessionIntValue("SelectedToDoCatId", 0); }
            set { SaveSessionValue(value, "SelectedToDoCatId"); }
        }

        public static int SelectedListId
        {
            get { return GetSessionIntValue("SelectedToDoListId", 0); }
            set { SaveSessionValue(value, "SelectedToDoListId"); }
        }

        public static int SelectedTaskId
        {
            get { return GetSessionIntValue("SelectedToDoTaskId", 0); }
            set { SaveSessionValue(value, "SelectedToDoTaskId"); }
        }

        public static int DeletedTaskId
        {
            get { return GetSessionIntValue("DeletedToDoTaskId", 0); }
            set { SaveSessionValue(value, "DeletedToDoTaskId"); }
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
            HttpSessionState session = HttpContext.Current.Session;
            if (session == null) return;

            session[keyName] = obj;
        }
    }
}
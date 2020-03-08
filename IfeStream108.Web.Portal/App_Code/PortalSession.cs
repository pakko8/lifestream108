using LifeStream108.Libs.Entities;
using LifeStream108.Modules.UserManagement.Managers;
using System.Web;

namespace LifeStream108.Web.Portal.App_Code
{
    public class PortalSession
    {
        public User User { get; private set; }

        public static PortalSession Current
        {
            get
            {
                object sessionObject = HttpContext.Current.Session["AppSession"];
                if (sessionObject != null)
                {
                    return (PortalSession)sessionObject;
                }
                else
                {
                    // TODO Redirect to Login page
                    PortalSession newSession = new PortalSession();
                    newSession.User = UserManager.GetUser(1);
                    HttpContext.Current.Session["AppSession"] = newSession;
                    return newSession;
                }
            }
        }
    }
}

using LifeStream108.Web.Portal.App_Code;
using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace LifeStream108.Web.Portal
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Session_Start(object sender, EventArgs e)
        {
        }

        void Application_BeginRequest()
        {
            PortalSession.LastErrorMessage = "";
        }
    }
}

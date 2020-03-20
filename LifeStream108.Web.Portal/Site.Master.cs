using LifeStream108.Web.Portal.App_Code;
using System;
using System.Web.UI;

namespace LifeStream108.Web.Portal
{
    public partial class SiteMaster : MasterPage
    {
        protected bool authorized;

        protected void Page_Init(object sender, EventArgs e)
        {
            authorized = PortalSession.IsAuthorized;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.AppRelativeVirtualPath.ToUpper().EndsWith("LOGIN.ASPX") && !PortalSession.IsAuthorized)
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}

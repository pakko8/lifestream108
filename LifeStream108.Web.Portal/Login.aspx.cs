using LifeStream108.Web.Portal.App_Code;
using System;
using System.Web.UI;

namespace LifeStream108.Web.Portal
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            bool authFlag = PortalSession.AuthorizeUser(email, password);

            if (authFlag) Response.Redirect("Default.aspx");
        }
    }
}

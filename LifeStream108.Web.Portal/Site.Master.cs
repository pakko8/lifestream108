using LifeStream108.Web.Portal.App_Code;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

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
            ShowLastMessage();
            PortalSession.SetLastMessage("", LastMessageType.None);

            if (!Page.AppRelativeVirtualPath.ToUpper().EndsWith("LOGIN.ASPX") && !PortalSession.IsAuthorized)
            {
                Response.Redirect("Login.aspx");
            }
        }

        private void ShowLastMessage()
        {
            string message = PortalSession.LastMessage;
            if (string.IsNullOrEmpty(message))
            {
                divAlert.Visible = false;
                return;
            }

            string cssPart; // https://getbootstrap.com/docs/4.0/components/alerts/
            switch (PortalSession.LastMessageType)
            {
                case LastMessageType.Success:
                    cssPart = "success";
                    break;
                case LastMessageType.Info:
                    cssPart = "info";
                    break;
                case LastMessageType.Warning:
                    cssPart = "warning";
                    break;
                case LastMessageType.Error:
                    cssPart = "danger";
                    break;
                default:
                    cssPart = "secondary";
                    break;
            }
            // <div class="alert alert-success" role="alert">Message</div>
            divAlert.Visible = true;
            HtmlGenericControl newDiv = new HtmlGenericControl("DIV");
            newDiv.Attributes.Add("class", "alert alert-" + cssPart);
            newDiv.InnerText = message;
            divAlert.Controls.Add(new HtmlGenericControl("BR"));
            divAlert.Controls.Add(newDiv);
        }
    }
}

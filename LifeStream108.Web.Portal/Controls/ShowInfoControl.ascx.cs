using LifeStream108.Web.Portal.App_Code;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace LifeStream108.Web.Portal.Controls
{
    public partial class ShowInfoControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void SetMessage(string message, LastMessageType type)
        {
            divAlert.Visible = true;
            string cssPart; // https://getbootstrap.com/docs/4.0/components/alerts/
            switch (type)
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
            HtmlGenericControl newDiv = new HtmlGenericControl("DIV");
            newDiv.Attributes.Add("class", "alert alert-" + cssPart);
            newDiv.InnerText = message;
            divAlert.Controls.Add(new HtmlGenericControl("BR"));
            divAlert.Controls.Add(newDiv);
        }
    }
}

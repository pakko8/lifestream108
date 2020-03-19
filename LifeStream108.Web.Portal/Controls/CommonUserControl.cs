using System.Web.UI;

namespace LifeStream108.Web.Portal.Controls
{
    public class CommonUserControl : UserControl
    {
        protected ShowInfoControl ShowInfoControl
        {
            get { return (ShowInfoControl)Parent.FindControl("showInfoControl"); }
        }
    }
}
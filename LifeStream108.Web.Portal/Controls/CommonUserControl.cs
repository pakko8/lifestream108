using NLog;
using System.Web.UI;

namespace LifeStream108.Web.Portal.Controls
{
    public class CommonUserControl : UserControl
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected ShowInfoControl ShowInfoControl
        {
            get { return (ShowInfoControl)Parent.FindControl("showInfoControl"); }
        }
    }
}
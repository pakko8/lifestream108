using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement.Managers;
using LifeStream108.Web.Portal.App_Code;
using NLog;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LifeStream108.Web.Portal
{
    public partial class _Default : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            Logger.Info("Test");
            try
            {
                ToDoCategory[] categories = ToDoCategoryManager.GetUserCategories(PortalSession.Current.User.Id);
                foreach (ToDoCategory cat in categories)
                {
                    Logger.Info(cat.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
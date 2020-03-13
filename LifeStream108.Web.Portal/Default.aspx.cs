using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement.Managers;
using LifeStream108.Web.Portal.App_Code;
using NLog;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LifeStream108.Web.Portal
{
    public partial class _Default : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                Logger.Error("Error loading page: " + ex);
            }
        }

        private void LoadData()
        {
            categoriesControl.LoadCaterories();
         }

        protected void categoriesControl_CategoryChanged()
        {
            listsControl.LoadLists();
        }
    }
}

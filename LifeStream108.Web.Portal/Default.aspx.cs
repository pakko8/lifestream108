using LifeStream108.Web.Portal.App_Code;
using LifeStream108.Web.Portal.Controls;
using NLog;
using System;
using System.Web.UI;

namespace LifeStream108.Web.Portal
{
    public partial class _Default : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                categoriesControl.LoadCategories();
                listsControl.LoadLists();
                tasksControl.LoadTasks();
                taskInfoControl.Visible = PortalSession.SelectedTaskId > 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Error loading page: " + ex);
            }
        }

        public ToDoTasksControl TaskListControl => tasksControl;
    }
}

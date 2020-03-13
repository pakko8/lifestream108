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
                taskInfoControl.LoadTaskInfo();
            }
            catch (Exception ex)
            {
                Logger.Error("Error loading page: " + ex);
            }
        }
    }
}

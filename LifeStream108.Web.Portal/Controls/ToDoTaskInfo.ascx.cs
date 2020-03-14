using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Web.Portal.App_Code;
using System;
using System.Linq;
using System.Web.UI;

namespace LifeStream108.Web.Portal.Controls
{
    public partial class ToDoTaskInfo : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void LoadTaskInfo()
        {
            int taskId = WebUtils.GetRequestIntValue(Constants.RequestTaskKeyName, Request, 0);
            ToDoTask[] tasks = PortalSession.ToDoTasks;
            ToDoTask task = tasks.FirstOrDefault(n => n.Id == taskId);

            txtTitle.Text = task.Title;
            txtNote.Text = task.Note;
        }
    }
}
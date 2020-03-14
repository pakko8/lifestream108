using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement.Managers;
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
            if (!Page.IsPostBack)
            {
                LoadTaskInfo();
            }
        }

        public void LoadTaskInfo()
        {
            int taskId = WebUtils.GetRequestIntValue(Constants.RequestTaskKeyName, Request, 0);
            ToDoTask[] tasks = PortalSession.ToDoTasks;
            ToDoTask task = tasks.FirstOrDefault(n => n.Id == taskId);
            if (task == null) return;

            PortalSession.SelectedTaskId = task.Id;

            txtTitle.Text = task.Title;
            txtNote.Text = task.Note;
        }

        protected void btnSaveTask_Click(object sender, EventArgs e)
        {
            int taskId = PortalSession.SelectedTaskId;
            ToDoTask task = ToDoTaskManager.GetTask(taskId);
            task.Title = txtTitle.Text;
            task.Note = txtNote.Text;
            ToDoTaskManager.UpdateTask(task);

            UpdateTaskInSession(task);
        }

        private void UpdateTaskInSession(ToDoTask task)
        {
            ToDoTask[] tasks = PortalSession.ToDoTasks;
            int index = Array.IndexOf(tasks, task);
            tasks[index] = task;
            PortalSession.ToDoTasks = tasks;
        }

        protected void btnDeleteTask_Click(object sender, EventArgs e)
        {
            int taskId = PortalSession.SelectedTaskId;
            ToDoTask task = ToDoTaskManager.GetTask(taskId);
            task.Status = ToDoTaskStatus.Deleted;
            ToDoTaskManager.UpdateTask(task);

            ToDoTask[] tasks = PortalSession.ToDoTasks;
            tasks = tasks.Where(n => n.Id != task.Id).ToArray();
            PortalSession.ToDoTasks = tasks;

            PortalSession.SelectedTaskId = 0;
            PortalSession.DeletedTaskId = task.Id;
            Visible = false;
        }
    }
}

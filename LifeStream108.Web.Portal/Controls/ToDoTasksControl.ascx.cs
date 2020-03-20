using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement.Managers;
using LifeStream108.Web.Portal.App_Code;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace LifeStream108.Web.Portal.Controls
{
    public partial class ToDoTasksControl : CommonUserControl
    {
        private const int TaskTitleMaxLength = 80;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void LoadTasks()
        {
            divTasks.Controls.Clear();

            ToDoTask[] foundTasks = PortalSession.ToDoTasksFound;
            if (foundTasks != null && foundTasks.Length > 0)
            {
                AddTasksToControl(foundTasks, 0);
                return;
            }

            int currentListId = PortalSession.SelectedListId;
            if (currentListId <= 0) return;

            ToDoTask[] tasksToShow = GetTasks(currentListId);
            int selectedTaskId = GetSelectedTaskId(tasksToShow);
            AddTasksToControl(tasksToShow, selectedTaskId);
        }

        private void AddTasksToControl(ToDoTask[] tasks, int selectedTaskId)
        {
            foreach (ToDoTask task in tasks)
            {
                HyperLink taskLink = new HyperLink
                {
                    ID = "btnTask_" + task.Id,
                    NavigateUrl = $"/Default?{Constants.RequestTaskKeyName}={task.Id}",
                    Enabled = selectedTaskId != task.Id,
                    CssClass = selectedTaskId == task.Id ? "btn btn-info" : "btn btn-secondary"
                };
                string taskTitle = task.Title.Length > TaskTitleMaxLength
                    ? task.Title.Substring(0, TaskTitleMaxLength) + " ..."
                    : task.Title;
                taskLink.Controls.Add(new Literal { Text = $"<span class=\"pull-left\">{taskTitle}</span>&nbsp;" });
                divTasks.Controls.Add(taskLink);
            }
        }

        private ToDoTask[] GetTasks(int currentListId)
        {
            ToDoTask[] taskArray = PortalSession.ToDoTasks;

            bool needLoadTasksFromDb = true;
            if (taskArray != null && taskArray.Length > 0)
            {
                needLoadTasksFromDb = taskArray[0].ListId != currentListId;
            }

            if (needLoadTasksFromDb)
            {
                taskArray = ToDoTaskManager.GetListTasks(currentListId).Where(n => n.Status != ToDoTaskStatus.Deleted).ToArray();
                PortalSession.ToDoTasks = taskArray;
            }

            return taskArray;
        }

        private int GetSelectedTaskId(ToDoTask[] tasks)
        {
            int selectedTaskId = WebUtils.GetRequestIntValue(Constants.RequestTaskKeyName, Request, 0);
            if (selectedTaskId <= 0) selectedTaskId = PortalSession.SelectedTaskId;

            ToDoTask foundTask = tasks.FirstOrDefault(n => n.Id == selectedTaskId);
            if (foundTask == null) return 0;

            PortalSession.SelectedTaskId = selectedTaskId;
            return selectedTaskId;
        }
    }
}

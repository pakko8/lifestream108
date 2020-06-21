using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement;
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
            foreach (ToDoTask task in tasks.OrderByDescending(n => n.RegTime))
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
                taskArray = ToDoTaskManager.GetListActiveTasks(currentListId);
                PortalSession.ToDoTasks = taskArray;
            }

            return taskArray;
        }

        private int GetSelectedTaskId(ToDoTask[] tasks)
        {
            int selectedTaskId = App_Code.WebUtils.GetRequestIntValue(Constants.RequestTaskKeyName, Request, 0);
            if (selectedTaskId <= 0) selectedTaskId = PortalSession.SelectedTaskId;

            ToDoTask foundTask = tasks.FirstOrDefault(n => n.Id == selectedTaskId);
            if (foundTask == null) return 0;

            PortalSession.SelectedTaskId = selectedTaskId;
            return selectedTaskId;
        }

        protected void btnAddNewTask_Click(object sender, EventArgs e)
        {
            int selectedListId = PortalSession.SelectedListId;
            if (selectedListId == 0)
            {
                ShowInfoControl.SetMessage("Не выбран список", LastMessageType.Error);
                return;
            }

            string newTitle = txtNewTaskTitle.Text.Trim();
            ToDoTask[] tasks = PortalSession.ToDoTasks;
            ToDoTask existsTask = tasks.FirstOrDefault(n => n.Title.ToUpper() == newTitle.ToUpper());
            if (existsTask != null)
            {
                ShowInfoControl.SetMessage($"Задача с заголовком '{newTitle}' уже существует", LastMessageType.Error);
                return;
            }

            ToDoTask newTask = new ToDoTask
            {
                UserId = PortalSession.User.Id,
                ListId = selectedListId,
                Title = newTitle
            };
            ToDoTaskManager.AddTask(newTask);
            PortalSession.ToDoTasks = CollectionUtils.Merge(tasks, newTask);
            txtNewTaskTitle.Text = "";
            LoadTasks();
        }
    }
}

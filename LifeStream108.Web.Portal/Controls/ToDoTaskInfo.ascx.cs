using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement.Managers;
using LifeStream108.Web.Portal.App_Code;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LifeStream108.Web.Portal.Controls
{
    public partial class ToDoTaskInfo : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                foreach (ReminderRepeaterType reminderType in Enum.GetValues(typeof(ReminderRepeaterType)))
                {
                    ListItem listItem = new ListItem
                    {
                        Value = reminderType.ToString(),
                        Text = reminderType.ToString()
                    };
                    ddlReminderRepeatType.Items.Add(listItem);
                }
                LoadTaskInfo();
            }
        }

        public void LoadTaskInfo()
        {
            int taskId = WebUtils.GetRequestIntValue(Constants.RequestTaskKeyName, Request, 0);
            ToDoTask[] tasks = PortalSession.ToDoTasks;
            ToDoTask task = tasks.FirstOrDefault(n => n.Id == taskId);
            if (task == null) return;

            ToDoTaskReminder reminder = new ToDoTaskReminder();
            reminder.Load(task.ReminderSettings);
            if (reminder.Time != DateTime.MinValue)
            {
                txtReminderTime.Text = reminder.UserFormattedTime;
                if (reminder.RepeaterValue > 0) txtReminderRepeatValue.Text = reminder.RepeaterValue.ToString();
                ddlReminderRepeatType.SelectedValue = reminder.RepeaterType.ToString();

            }
            PortalSession.SelectedTaskId = task.Id;

            txtTitle.Text = task.Title;
            txtNote.Text = task.Note;
        }

        protected void btnSaveTask_Click(object sender, EventArgs e)
        {
            int taskId = PortalSession.SelectedTaskId;
            ToDoTask task = ToDoTaskManager.GetTask(taskId);

            if (!string.IsNullOrEmpty(txtReminderTime.Text))
            {
                ToDoTaskReminder reminder = new ToDoTaskReminder();
                int repeatValue = int.Parse(txtReminderRepeatValue.Text);
                //reminder.Load(txtReminderTime.Text.Trim(), )
            }
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

using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.Entities.ToDoEntities.Reminders;
using LifeStream108.Modules.ToDoListManagement.Managers;
using LifeStream108.Web.Portal.App_Code;
using NLog;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LifeStream108.Web.Portal.Controls
{
    public partial class ToDoTaskInfoControl : CommonUserControl
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
            ToDoTask[] tasks = PortalSession.ToDoTasksFound;
            if (tasks == null) tasks = PortalSession.ToDoTasks;
            if (tasks == null) return;

            ToDoTask task = tasks.FirstOrDefault(n => n.Id == taskId);
            if (task == null) return;

            ShowTaskInfo(task);
            PortalSession.SelectedTaskId = task.Id;
        }

        private void ShowTaskInfo(ToDoTask task)
        {
            var createReminderResult = Reminder.Create(task.ReminderSettings);
            if (createReminderResult.Reminder.Time != DateTime.MinValue)
            {
                txtReminderTime.Text = createReminderResult.Reminder.UserFormattedTime;
                if (createReminderResult.Reminder.RepeaterValue > 0) txtReminderRepeatValue.Text = createReminderResult.Reminder.RepeaterValue.ToString();
                ddlReminderRepeatType.SelectedValue = createReminderResult.Reminder.RepeaterType.ToString();

            }

            txtTitle.Text = task.Title;
            txtNote.Text = task.Note;
        }

        protected void btnSaveTask_Click(object sender, EventArgs e)
        {
            int taskId = 0;
            try
            {
                taskId = PortalSession.SelectedTaskId;
                Logger.Info("Updating task " + taskId);
                ToDoTask task = ToDoTaskManager.GetTask(taskId);

                if (!string.IsNullOrEmpty(txtReminderTime.Text))
                {
                    var createReminderResult = Reminder.Create(
                        txtReminderTime.Text.Trim(), txtReminderRepeatValue.Text, ddlReminderRepeatType.SelectedValue);
                    if (!string.IsNullOrEmpty(createReminderResult.Error))
                    {
                        ShowInfoControl.SetMessage(createReminderResult.Error, LastMessageType.Error);
                        return;
                    }

                    task.ReminderSettings = createReminderResult.Reminder.ReminderFormat;
                }

                string newTitle = txtTitle.Text.Trim();
                if (newTitle != task.Title) Logger.Info($"Prev title: '{task.Title}'");
                task.Title = newTitle;

                string newNote = txtNote.Text.Trim();
                if (newNote != task.Note) Logger.Info($"Prev note: '{task.Note}'");
                task.Note = newNote;
                ToDoTaskManager.UpdateTask(task);

                UpdateTaskInSession(task);

                ShowInfoControl.SetMessage("Задача сохранена", LastMessageType.Success);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating task with id {taskId}: {ex}");
                ShowInfoControl.SetMessage("Ошибка сохранения задачи: " + ex.Message, LastMessageType.Error);
            }
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
            Logger.Info($"Deactivating task " + taskId);
            ToDoTask task = ToDoTaskManager.GetTask(taskId);
            Logger.Info($"Title='{task.Title}', note='{task.Note}'");
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

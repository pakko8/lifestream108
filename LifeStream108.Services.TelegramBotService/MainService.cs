using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.Entities.ToDoEntities.Reminders;
using LifeStream108.Libs.Entities.UserEntities;
using LifeStream108.Modules.LifeActivityManagement;
using LifeStream108.Modules.SettingsManagement;
using LifeStream108.Modules.TelegramBotManager;
using LifeStream108.Modules.ToDoListManagement;
using LifeStream108.Modules.UserManagement;
using NLog;
using System;
using System.Linq;
using System.ServiceProcess;

namespace LifeStream108.Services.TelegramBotService
{
    internal class MainService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly System.Timers.Timer _clearSessionsTimer;
        private readonly System.Timers.Timer _checkToDoTaskReminders;
        private readonly System.Timers.Timer _checkActivityLogs;

        private MainTelegramChatClient _chatClient = null;

        public MainService()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                _clearSessionsTimer = new System.Timers.Timer();
                _clearSessionsTimer.Interval = 10 * 60 * 1000;
                _clearSessionsTimer.Elapsed += ClearSessionsTimer_Elapsed;

                _checkToDoTaskReminders = new System.Timers.Timer();
                _checkToDoTaskReminders.Interval = 3 * 60 * 1000;
                _checkToDoTaskReminders.Elapsed += CheckToDoTaskReminders_Elapsed;

                _checkActivityLogs = new System.Timers.Timer();
                _checkActivityLogs.Interval = 30 * 60 * 1000;
                _checkActivityLogs.Elapsed += CheckActivityLogs_Elapsed;
            }
            catch (Exception ex)
            {
                Logger.Error("Constructor Exception: {0}", ex.ToString());
            }
        }

        private void CheckActivityLogs_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Info("Check activity logs started");
            try
            {
                _checkActivityLogs.Stop();

                DateTime now = DateTime.Now;
                foreach (User user in UserManager.GetAllUsers().Where(n => n.Status == UserStatus.Active))
                {
                    // Делаем эту проверку для каждого пользователя на случай,
                    // если по какой-либо ошибке проверка не сработала ля всех пользователей
                    if (!(now.Hour >= 8 && now.Hour <= 12 && (now - user.CheckActLogsTime).TotalHours > 24)) continue;

                    LifeActivity[] acts = LifeActivityManager.GetActivitiesForUser(user.Id)
                        .Where(n => n.PeriodType != PeriodicityType.None).ToArray();
                    if (acts.Length == 0)
                    {
                        Logger.Info($"{user} has only usual activities");
                        continue;
                    }

                    if (now.Day == 1)
                    {

                    }
                    if (now.Month == 1 && now.Day == 1)
                    {

                    }
                    Logger.Info("We need check activity logs for " + user);
                    foreach (LifeActivity act in acts)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error to check activity logs: " + ex);
            }
            finally
            {
                _checkActivityLogs.Start();
            }
            Logger.Info("Check activity logs finished");
        }

        private void CheckToDoTaskReminders_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Info("Check reminders task started");
            try
            {
                _checkToDoTaskReminders.Stop();

                foreach (User user in UserManager.GetAllUsers().Where(n => n.Status == UserStatus.Active))
                {
                    Logger.Info("Check reminders for user " + user.Id);
                    ToDoList[] lists = ToDoListManager.GetUserLists(user.Id);
                    foreach (ToDoTask task in ToDoTaskManager.GetTasksWithActiveReminders(user.Id))
                    {
                        ToDoList list = lists.FirstOrDefault(n => n.Id == task.ListId);
                        if (!list.Active) continue;

                        var createReminderResult = Reminder.Create(task.ReminderSettings);
                        DateTime comingReminderTime = createReminderResult.Reminder.GetComingSoonReminderTime(task.ReminderLastTime);
                        Logger.Info($"Checking reminder '{task.ReminderSettings}' for task '{task.Title}'." +
                            $" Coming reminder time: {comingReminderTime:yyyy-MM-dd HH:mm}");
                        if (createReminderResult.Reminder.IsTimeToRemind(task.ReminderLastTime))
                        {
                            string reminderTaskInfo =
                                $"[{task.Id}] {task.Title}: {createReminderResult.Reminder.FormatReminderForUser(task.ReminderLastTime)}";
                            Logger.Info($"We'll send reminder ({task.ReminderSettings}) about task '{reminderTaskInfo}'");
                            task.ReminderLastTime = DateTime.Now;
                            ToDoTaskManager.UpdateTask(task);
                            SendMessage($"Напоминание о задаче: {reminderTaskInfo}", user.TelegramId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error to check reminders: " + ex);
            }
            finally
            {
                _checkToDoTaskReminders.Start();
            }
            Logger.Info("Check reminders task finished");
        }

        private void ClearSessionsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Info("Clear sessions task started");
            try
            {
                _clearSessionsTimer.Stop();

                ClearSessionsProcessor clearSessionsProcessor = new ClearSessionsProcessor();
                clearSessionsProcessor.Run(30); // TODO Move value to settings
            }
            catch (Exception ex)
            {
                Logger.Error("Error to clear sessions: " + ex);
            }
            finally
            {
                _clearSessionsTimer.Start();
            }
            Logger.Info("Clear sessions task finished");
        }

        private async void SendMessage(string message, int telegramUserId)
        {
            Logger.Info("Sending message: " + message);
            await _chatClient.BotClient.SendTextMessageAsync(telegramUserId, message, Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Service started");
            try
            {
                _chatClient = new MainTelegramChatClient();
                _chatClient.Start();
                _clearSessionsTimer.Start();
                _checkToDoTaskReminders.Start();
                _checkActivityLogs.Start();
            }
            catch (Exception ex)
            {
                Logger.Error("Service start error: " + ex);
            }
        }

        protected override void OnStop()
        {
            _clearSessionsTimer.Stop();
            _chatClient.Stop();
            _clearSessionsTimer.Stop();
            _checkToDoTaskReminders.Stop();
            _checkActivityLogs.Start();

            Logger.Info("Service stopped");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Logger.Error("Service crash error: " + ex);
        }
    }
}

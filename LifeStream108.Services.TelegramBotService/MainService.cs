using LifeStream108.Libs.Entities.NewsEntities;
using LifeStream108.Libs.Entities.UserEntities;
using LifeStream108.Modules.NewsManagement.Managers;
using LifeStream108.Modules.NewsProcessors;
using LifeStream108.Modules.TelegramBotManager;
using LifeStream108.Modules.UserManagement.Managers;
using NLog;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace LifeStream108.Services.TelegramBotService
{
    internal class MainService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly System.Timers.Timer _clearSessionsTimer;
        private readonly System.Timers.Timer _checkNewsTimer;

        private MainTelegramChatClient _chatClient = null;

        public MainService()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                _clearSessionsTimer = new System.Timers.Timer();
                _clearSessionsTimer.Interval = 10 * 60 * 1000;
                _clearSessionsTimer.Elapsed += ClearSessionsTimer_Elapsed;

                _checkNewsTimer = new System.Timers.Timer();
                _checkNewsTimer.Interval = 10 * 60 * 1000;
                _checkNewsTimer.Elapsed += CheckNewsTimer_Elapsed;
            }
            catch (Exception ex)
            {
                Logger.Error("Constructor Exception: {0}", ex.ToString());
            }
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

        private void CheckNewsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Info("Check news task started");
            try
            {
                _checkNewsTimer.Stop();

                NewsGroup[] newsGroups = NewsGroupManager.GetAllActiveGroups();
                NewsProcessorLoader processorLoader = new NewsProcessorLoader();
                StringBuilder sbMessage = new StringBuilder();
                foreach (NewsGroup group in newsGroups.OrderBy(n => n.Priority))
                {
                    if ((DateTime.Now - group.LastRunTime).TotalMinutes < group.CheckIntervalInMinutes) continue;

                    BaseNewsProcessor processor = processorLoader.LoadClass(group.ProcessorClassName);
                    NewsItem lastNewsItem = NewsItemManager.GetLastNewsItem(group.Id);
                    NewsItem[] newsList = processor.GetLastNews(group.Url, group.Id,
                        lastNewsItem != null ? lastNewsItem.NewsTime.AddHours(-1) : DateTime.Now.AddDays(-7));
                    int countNews = 0;
                    foreach (NewsItem newsItem in newsList)
                    {
                        NewsItem existNewsItem = NewsItemManager.GetNewsItemByResourceId(newsItem.ResourceId);
                        if (existNewsItem != null) continue;

                        NewsItemManager.AddNewsItem(newsItem);
                        countNews++;
                        if (countNews == 1)
                        {
                            if (sbMessage.Length > 0) sbMessage.Append("\r\n");
                            sbMessage.Append($"В группе <b>{group.Name}</b> {(newsList.Length > 1 ? "свежие новости" : "свежая новость")}:\r\n");
                        }
                        sbMessage.Append($@"<a href=""{newsItem.Url}"">{newsItem.Title}</a>");
                        sbMessage.Append("\r\n");
                    }

                    group.LastRunTime = DateTime.Now;
                    group.RunStatus = NewsGroupRunStatus.Success;
                    NewsGroupManager.UpdateGroup(group);
                }

                if (sbMessage.Length > 0)
                {
                    User superuser = UserManager.GetSuperuser();
                    try
                    {
                        SendMessage(sbMessage.ToString(), superuser.TelegramId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error to send message with news: " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error to check news: " + ex);
            }
            finally
            {
                _checkNewsTimer.Start();
            }
            Logger.Info("Check news task finished");
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
                _checkNewsTimer.Start();
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
            _checkNewsTimer.Stop();
            Logger.Info("Service stopped");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Logger.Error("Service crash error: " + ex);
        }
    }
}

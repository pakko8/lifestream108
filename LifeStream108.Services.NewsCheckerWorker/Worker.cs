using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LifeStream108.Libs.Entities.NewsEntities;
using LifeStream108.Libs.Entities.UserEntities;
using LifeStream108.Modules.NewsManagement;
using LifeStream108.Modules.NewsProcessors;
using LifeStream108.Modules.SettingsManagement;
using LifeStream108.Modules.UserManagement;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace LifeStream108.Services.NewsCheckerWorker
{
    public class Worker : BackgroundService
    {
        private readonly AppSettings _appSettings;
        private HttpClient _httpClient;

        public Worker(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                InnerHelpers.Logger.Info("Timer started.");
                try
                {
                    User superuser = UserManager.GetSuperuser();

                    NewsGroup[] newsGroups = NewsGroupManager.GetAllActiveGroups();
                    NewsProcessorLoader processorLoader = new NewsProcessorLoader();
                    StringBuilder sbMessage = new StringBuilder();
                    foreach (NewsGroup group in newsGroups.OrderBy(n => n.Priority))
                    {
                        if ((DateTime.Now - group.LastRunTime).TotalMinutes < group.CheckIntervalInMinutes) continue;

                        BaseNewsProcessor processor = processorLoader.LoadClass(group.ProcessorClassName);
                        NewsHistoryItem lastNewsItem = NewsHisoryManager.GetLastHistoryItem(group.Id, superuser.Id);
                        NewsHistoryItem[] newsList = processor.GetLastNews(group.Url, group.Id,
                            lastNewsItem != null ? lastNewsItem.NewsTime.AddHours(-1) : DateTime.Now.AddDays(-7));
                        int countNews = 0;
                        foreach (NewsHistoryItem newsItem in newsList)
                        {
                            NewsHistoryItem existNewsItem = NewsHisoryManager.GetHistoryItemByResourceId(newsItem.ResourceId, superuser.Id);
                            if (existNewsItem != null) continue;

                            NewsHisoryManager.AddHistoryItem(newsItem, superuser.Id);
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
                        SendMessage(sbMessage.ToString(), superuser.TelegramId);
                    }
                }
                catch (Exception ex)
                {
                    InnerHelpers.Logger.Error("Process error: " + ex);
                }
				InnerHelpers.Logger.Info("Timer finished.");
                await Task.Delay(_appSettings.TimerIntervalInMinutes * 60 * 1000, stoppingToken);
            }
        }

        private async void SendMessage(string message, int telegramUserId)
        {
            try
            {
                SettingEntry botTokenSetting = SettingsManager.GetSettingEntryByCode(SettingCode.TelegramBotToken);
                TelegramBotClient botClient = new TelegramBotClient(botTokenSetting.Value);

                InnerHelpers.Logger.Info("Sending message: " + message);
                await botClient.SendTextMessageAsync(telegramUserId, message, Telegram.Bot.Types.Enums.ParseMode.Html);
            }
            catch (Exception ex)
            {
                InnerHelpers.Logger.Error("Error to send message with news: " + ex);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            InnerHelpers.Logger.Info("Service started.");
            _httpClient = new HttpClient();

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            InnerHelpers.Logger.Info("Service stopped.");
            if (_httpClient != null) _httpClient.Dispose();

            return base.StopAsync(cancellationToken);
        }
    }
}

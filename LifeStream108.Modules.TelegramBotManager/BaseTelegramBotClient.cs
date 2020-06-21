using LifeStream108.Modules.SettingsManagement;
using NLog;
using System;
using Telegram.Bot;
using TelegramUser = Telegram.Bot.Types.User;

namespace LifeStream108.Modules.TelegramBotManager
{
    public abstract class BaseTelegramBotClient
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected TelegramBotClient _botClient = null;

        public TelegramBotClient BotClient => _botClient;

        public virtual void Start()
        {
            SettingEntry botTokenSetting = SettingsManager.GetSettingEntryByCode(SettingCode.TelegramBotToken);

            /*WebProxy proxy = new WebProxy("159.89.23.103:8000", true);
            proxy.Credentials = new NetworkCredential("alex", "12345Aa");*/
            _botClient = new TelegramBotClient(botTokenSetting.Value/*, proxy*/);
            TelegramUser botUser = _botClient.GetMeAsync().Result;
            _botClient.StartReceiving();
            Logger.Info($"Bot <name={botUser.FirstName}, id={botUser.Id}, token={botTokenSetting.Value}> started listening");
        }

        public virtual void Stop()
        {
            if (_botClient != null)
            {
                try
                {
                    _botClient.StopReceiving();
                }
                catch (Exception ex)
                {
                    Logger.Error("Error to stop Telegram bot: " + ex);
                }
                _botClient = null;
            }
        }
    }
}

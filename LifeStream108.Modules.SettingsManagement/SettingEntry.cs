namespace LifeStream108.Modules.SettingsManagement
{
    public class SettingEntry
    {
        public virtual int Id { get; set; }

        public virtual SettingCode Code { get; set; }

        public virtual string Value { get; set; }

        public virtual string Options { get; set; } = "";
    }

    public enum SettingCode
    {
        MainDbConnString,

        /// <summary>
        /// Токен Телеграм бота для доступа к API
        /// </summary>
        TelegramBotToken,

        /// <summary>
        /// Директория с библиотеками комманд для Телеграмм бота
        /// </summary>
        BotCommandsDirectory
    }
}

using System;

namespace LifeStream108.Libs.Entities.SettingsEntities
{
    public class SettingEntry
    {
        public virtual int Id { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual SettingCode Code { get; set; }

        public virtual string Description { get; set; }

        public virtual DataType DataType { get; set; }

        public virtual string Value { get; set; }

        public virtual string Options { get; set; }

        public virtual DateTime RegTime { get; set; }
    }

    public enum SettingCode
    {
        TelegramBotToken,

        BotCommandsDirectory
    }
}

using LiteDB;
using System.Collections.Generic;
using System.IO;

namespace LifeStream108.Modules.SettingsManagement
{
    public static class SettingsManager
    {
        private const string LiteDbConnString = @"C:\_Projects\LiteDb\Settings.db";

        public static SettingEntry GetSettingEntryByCode(SettingCode code)
        {
            SettingEntry setting = null;
            using (var connection = new LiteDatabase(CreateReadolyLiteDbConnObj()))
            {
                var table = connection.GetCollection<SettingEntry>("Settings");
                setting = table.FindOne(n => n.Code == code);
            }
            return setting;
        }

        public static void CreateDb()
        {
            List<SettingEntry> allSettings = new List<SettingEntry>();
            allSettings.Add(new SettingEntry
            {
                Code = SettingCode.MainDbConnString,
                Value = "Server=127.0.0.1;Port=5432;Database=lifestream108;UserId=postgres;Password=123456;Timeout=1024"
            });
            allSettings.Add(new SettingEntry
            {
                Code = SettingCode.TelegramBotToken,
                Value = "1013133057:AAFPqHM2bWt3FA70HLT7es58BseG5hxd4X4"
            });
            allSettings.Add(new SettingEntry
            {
                Code = SettingCode.BotCommandsDirectory,
                Value = @"C:\_Projects\_CommandProcessors"
            });
            List<SettingEntry> settingsToSave = new List<SettingEntry>();
            if (File.Exists(LiteDbConnString))
            {
                using (var connection = new LiteDatabase(CreateReadolyLiteDbConnObj()))
                {
                    var table = connection.GetCollection<SettingEntry>("Settings");
                    foreach (SettingEntry setting in allSettings)
                    {
                        SettingEntry existSetting = table.FindOne(n => n.Code == setting.Code);
                        if (existSetting == null) settingsToSave.Add(setting);
                    }
                }
            }
            else
            {
                settingsToSave = allSettings;
            }

            if (settingsToSave.Count > 0)
            {
                using (var connection = new LiteDatabase(LiteDbConnString))
                {
                    var table = connection.GetCollection<SettingEntry>("Settings");
                    table.InsertBulk(settingsToSave);
                }
            }
        }

        private static ConnectionString CreateReadolyLiteDbConnObj()
        {
            return new ConnectionString
            {
                Filename = LiteDbConnString,
                Connection = ConnectionType.Shared
            };
        }
    }
}

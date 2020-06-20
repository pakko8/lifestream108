using LifeStream108.Libs.Entities.MessageEntities;
using LifeStream108.Libs.LiteDbHelper;
using LiteDB;
using System.IO;
using System.Linq;

namespace LifeStream108.Modules.TempDataManagement
{
    public static class TelegramMessageHistoryManager
    {
        private const string LiteDbDirectory = @"C:\_Projects\LiteDb\";

        public static TelegramMessageHistory[] GetHistoryForUser(int userId)
        {
            TelegramMessageHistory[] messages;
            var connInfo = GetConnString(userId);
            using (var connection = new LiteDatabase(LiteDbUtils.CreateReadolyConnection(connInfo.DbConnString)))
            {
                var table = connection.GetCollection<TelegramMessageHistory>(connInfo.TableName);
                messages = table.FindAll().ToArray();
            }
            return messages;
        }

        public static void AddHistory(TelegramMessageHistory message, int userId)
        {
            var connInfo = GetConnString(userId);
            using (var connection = new LiteDatabase(connInfo.DbConnString))
            {
                var table = connection.GetCollection<TelegramMessageHistory>(connInfo.TableName);
                table.Insert(message);
            }
        }

        public static void DeleteHistory(TelegramMessageHistory[] messages, int userId)
        {
            var connInfo = GetConnString(userId);
            using (var connection = new LiteDatabase(LiteDbUtils.CreateReadolyConnection(connInfo.DbConnString)))
            {
                var table = connection.GetCollection<TelegramMessageHistory>(connInfo.TableName);
                foreach (TelegramMessageHistory item in messages)
                {
                    table.Delete(item.Id);
                }
            }
        }

        private static (string DbConnString, string TableName) GetConnString(int userId)
        {
            string fileName = $"TelegramMsgHistory{LiteDbUtils.PrepateDbFileName(userId)}";
            return (Path.Combine(LiteDbDirectory, fileName), fileName);
        }
    }
}

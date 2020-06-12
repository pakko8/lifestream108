using LifeStream108.Libs.Entities.NewsEntities;
using LifeStream108.Libs.LiteDbHelper;
using LiteDB;
using System.IO;
using System.Linq;

namespace LifeStream108.Modules.NewsManagement
{
    public static class NewsHisoryManager
    {
        private const string LiteDbDirectory = @"C:\_Projects\LiteDb\NewsDbFiles";

        public static NewsHistoryItem GetHistoryItemByResourceId(string resourceId, int userId)
        {
            NewsHistoryItem foundItem = null;
            var connInfo = GetHistoryConnString(userId);
            using (var connection = new LiteDatabase(LiteDbUtils.CreateReadolyConnection(connInfo.DbConnString)))
            {
                var table = connection.GetCollection<NewsHistoryItem>(connInfo.TableName);
                foundItem = table.Find(n => n.ResourceId == resourceId).FirstOrDefault();
            }
            return foundItem;
        }

        public static NewsHistoryItem GetLastHistoryItem(int groupId, int userId)
        {
            NewsHistoryItem foundItem = null;
            var connInfo = GetHistoryConnString(userId);
            using (var connection = new LiteDatabase(LiteDbUtils.CreateReadolyConnection(connInfo.DbConnString)))
            {
                var table = connection.GetCollection<NewsHistoryItem>(connInfo.TableName);
                foundItem = table.Find(n => n.NewsGroupId == groupId).OrderByDescending(n => n.NewsTime).FirstOrDefault();
            }
            return foundItem;
        }

        public static void AddHistoryItem(NewsHistoryItem item, int userId)
        {
            var connInfo = GetHistoryConnString(userId);
            using (var connection = new LiteDatabase(connInfo.DbConnString))
            {
                var table = connection.GetCollection<NewsHistoryItem>(connInfo.TableName);
                table.Insert(item);
            }
        }

        private static (string DbConnString, string TableName) GetHistoryConnString(int userId)
        {
            string fileName = $"NewsHistory{LiteDbUtils.PrepateDbFileName(userId)}";
            return (Path.Combine(LiteDbDirectory, fileName), fileName);
        }
    }
}

using LifeStream108.Libs.Entities.NewsEntities;
using LiteDB;
using System.Linq;

namespace LifeStream108.Modules.NewsManagement
{
    public static class NewsHisoryManager
    {
        public static NewsHistoryItem GetHistoryItemByResourceId(string resourceId, int userId)
        {
            NewsHistoryItem foundItem = null;
            var connInfo = Helpers.GetLiteDbNewsHistoryConnString(userId);
            using (var connection = new LiteDatabase(Helpers.CreateReadolyLiteDbConnObj(connInfo.DbConnString)))
            {
                var table = connection.GetCollection<NewsHistoryItem>(connInfo.TableName);
                foundItem = table.Find(n => n.ResourceId == resourceId).FirstOrDefault();
            }
            return foundItem;
        }

        public static NewsHistoryItem GetLastHistoryItem(int groupId, int userId)
        {
            NewsHistoryItem foundItem = null;
            var connInfo = Helpers.GetLiteDbNewsHistoryConnString(userId);
            using (var connection = new LiteDatabase(Helpers.CreateReadolyLiteDbConnObj(connInfo.DbConnString)))
            {
                var table = connection.GetCollection<NewsHistoryItem>(connInfo.TableName);
                foundItem = table.Find(n => n.NewsGroupId == groupId).OrderByDescending(n => n.NewsTime).FirstOrDefault();
            }
            return foundItem;
        }

        public static void AddHistoryItem(NewsHistoryItem item, int userId)
        {
            var connInfo = Helpers.GetLiteDbNewsHistoryConnString(userId);
            using (var connection = new LiteDatabase(connInfo.DbConnString))
            {
                var table = connection.GetCollection<NewsHistoryItem>(connInfo.TableName);
                table.Insert(item);
            }
        }
    }
}

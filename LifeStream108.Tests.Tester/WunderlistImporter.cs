using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Modules.ToDoListManagement.Managers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LifeStream108.Tests.Tester
{
    /// <summary>
    /// https://developer.wunderlist.com/documentation/endpoints/list
    /// </summary>
    internal class WunderlistImporter
    {
        private const string Url = @"https://a.wunderlist.com/api/v1/";

        private const string FilesDirectory = @"D:\Temp\WunderlistFiles";

        private const int UserId = 1;

        private readonly int _categoryId;

        private readonly HttpClient _httpClient;
        private readonly WebClient _webClient;

        public WunderlistImporter(string clientId, string token, string categoryName, string email)
        {
            ToDoCategory category = new ToDoCategory
            {
                Name = categoryName,
                Email = email
            };
            ToDoCategoryManager.AddCategory(category);
            _categoryId = category.Id;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Access-Token", token);
            _httpClient.DefaultRequestHeaders.Add("X-Client-ID", clientId);
        }

        public void Run()
        {
            ToDoList[] lists = GetLists().Result;
            foreach (ToDoList list in lists)
            {
                ToDoTask[] tasks = GetListItems(list.Id).Result;
                if (tasks.Length == 0) continue;
                foreach (ToDoTask task in tasks)
                {

                }
            }
            _httpClient.Dispose();
        }

        private async Task<ToDoList[]> GetLists()
        {
            List<ToDoList> listArray = new List<ToDoList>();
            string responseContent = await _httpClient.GetStringAsync(Url + "lists");
            JArray jContent = JArray.Parse(responseContent);
            foreach (JToken jList in jContent)
            {
                ToDoList listInfo = new ToDoList();
                listInfo.Id = jList["id"].Value<int>();
                listInfo.CategoryId = _categoryId;
                listInfo.UserId = UserId;
                listInfo.Name = jList["title"].Value<string>();
                listInfo.RegTime = jList["created_at"].Value<DateTime>();

                DownloadListFiles(listInfo.Id);

                listArray.Add(listInfo);
            }
            return listArray.ToArray();
        }

        private async Task<ToDoTask[]> GetListItems(int listId)
        {
            (int TaskId, DateTime Reminder)[] reminders = GetListReminders(listId).Result;

            List<ToDoTask> taskArray = new List<ToDoTask>();
            string responseContent = await _httpClient.GetStringAsync(Url + "tasks?list_id=" + listId);
            JArray jContent = JArray.Parse(responseContent);
            foreach (JToken jList in jContent)
            {
                bool completed = jList["completed"].Value<bool>();
                if (completed) continue;

                ToDoTask taskInfo = new ToDoTask();
                taskInfo.Id = jList["id"].Value<int>();
                taskInfo.UserId = UserId;
                taskInfo.Title = jList["title"].Value<string>();
                taskInfo.RegTime = jList["created_at"].Value<DateTime>();
                taskInfo.Note = GetTaskNote(taskInfo.Id).Result;

                (int TaskId, DateTime Reminder) reminder = reminders.FirstOrDefault(n => n.TaskId == taskInfo.Id);
                if (reminder.ToTuple() != null)
                {
                    taskInfo.ReminderSettings = $"once{{{reminder.Reminder.ToString("yyyyMMddHHmmss")}}}";
                }

                taskArray.Add(taskInfo);
            }
            return taskArray.ToArray();
        }

        private async Task<string> GetTaskNote(int taskId)
        {
            string responseContent = await _httpClient.GetStringAsync(Url + "notes?task_id=" + taskId);
            JArray jContent = JArray.Parse(responseContent);
            StringBuilder sbNote = new StringBuilder();
            for (int i = 0; i < jContent.Count; i++)
            {
                JToken jNote = jContent[i];
                sbNote.Append(jNote["content"].Value<string>());
                if (i < jContent.Count - 1) sbNote.Append("<br>");
            }
            return sbNote.ToString();
        }

        private async Task<(int TaskId, DateTime Reminder)[]> GetListReminders(int listId)
        {
            string responseContent = await _httpClient.GetStringAsync(Url + "reminders?list_id=" + listId);
            JArray jContent = JArray.Parse(responseContent);
            List<(int TaskId, DateTime Reminder)> reminders = new List<(int TaskId, DateTime Reminder)>();
            foreach (JToken jReminder in jContent)
            {
                int taskId = jReminder["task_id"].Value<int>();
                DateTime time = jReminder["date"].Value<DateTime>();
                reminders.Add((taskId, time));
            }
            return reminders.ToArray();
        }

        private async void DownloadListFiles(int listId)
        {
            string responseContent = await _httpClient.GetStringAsync(Url + "files?list_id=" + listId);
            JArray jContent = JArray.Parse(responseContent);
            foreach (JToken jFile in jContent)
            {
                int taskId = jFile["task_id"].Value<int>();
                string fileUrl = jFile["url"].Value<string>();
                string fileType = jFile["content_type"].Value<string>().Replace("/", "-");
                string fileName = Path.Combine(FilesDirectory,
                    $"cat={_categoryId}_task={taskId}_type={fileType}_{jFile["file_name"].Value<string>()}");
                if (File.Exists(fileName)) File.Delete(fileName);

                _webClient.DownloadFile(fileUrl, fileUrl);
            }
        }
    }
}

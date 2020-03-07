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

            _webClient = new WebClient();
        }

        public void Run()
        {
            ToDoList[] lists = GetLists().Result;
            foreach (ToDoList list in lists)
            {
                (string FileUrl, string FileName)[] files = GetListFiles(list.Id).Result;
                DownloadFiles(files);

                ToDoTask[] tasks = GetListItems(list.Id).Result;
                if (tasks.Length == 0) continue;

                ToDoListManager.AddList(list);
                foreach (ToDoTask task in tasks)
                {
                    task.ListId = list.Id;
                    ToDoTaskManager.AddTask(task);
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

                listArray.Add(listInfo);
            }
            return listArray.ToArray();
        }

        private async Task<ToDoTask[]> GetListItems(int listId)
        {
            (long TaskId, string Note)[] notes = GetListNotes(listId).Result;
            (long TaskId, DateTime Reminder)[] reminders = GetListReminders(listId).Result;

            List<ToDoTask> taskArray = new List<ToDoTask>();
            string responseContent = await _httpClient.GetStringAsync(Url + "tasks?list_id=" + listId);
            JArray jContent = JArray.Parse(responseContent);
            foreach (JToken jList in jContent)
            {
                bool completed = jList["completed"].Value<bool>();
                if (completed) continue;

                ToDoTask taskInfo = new ToDoTask();
                long taskId = jList["id"].Value<long>();
                taskInfo.UserId = UserId;
                taskInfo.Title = jList["title"].Value<string>();
                taskInfo.RegTime = jList["created_at"].Value<DateTime>();
                taskInfo.Note = GetNote(taskId, notes);

                (long TaskId, DateTime Reminder) reminder = reminders.FirstOrDefault(n => n.TaskId == taskId);
                if (reminder.ToTuple() != null && reminder.Reminder.Year > 2019)
                {
                    taskInfo.ReminderSettings = $"once{{{reminder.Reminder.ToString("yyyyMMddHHmmss")}}}";
                }

                taskArray.Add(taskInfo);
            }
            return taskArray.ToArray();
        }

        private async Task<(long TaskId, string Note)[]> GetListNotes(int listId)
        {
            string responseContent = await _httpClient.GetStringAsync(Url + "notes?list_id=" + listId);
            JArray jContent = JArray.Parse(responseContent);
            List<(long TaskId, string Note)> notes = new List<(long TaskId, string Note)>();
            foreach (JToken jNote in jContent)
            {
                long taskId = jNote["task_id"].Value<long>();
                string note = jNote["content"].Value<string>();
                notes.Add((taskId, note));
            }
            return notes.ToArray();
        }

        private static string GetNote(long taskId, (long TaskId, string Note)[] notes)
        {
            (long TaskId, string Note)[] thisTaskNotes = notes.Where(n => n.TaskId == taskId).ToArray();
            StringBuilder sbNote = new StringBuilder();
            for (int i = 0; i < thisTaskNotes.Length; i++)
            {
                (long TaskId, string Note) note = thisTaskNotes[i];
                sbNote.Append(note.Note);
                if (i < thisTaskNotes.Length - 1) sbNote.Append("<br>");
            }
            return sbNote.ToString();
        }
        /*
         */
        private async Task<(long TaskId, DateTime Reminder)[]> GetListReminders(int listId)
        {
            string responseContent = await _httpClient.GetStringAsync(Url + "reminders?list_id=" + listId);
            JArray jContent = JArray.Parse(responseContent);
            List<(long TaskId, DateTime Reminder)> reminders = new List<(long TaskId, DateTime Reminder)>();
            foreach (JToken jReminder in jContent)
            {
                long taskId = jReminder["task_id"].Value<long>();
                DateTime time = jReminder["date"].Value<DateTime>();
                reminders.Add((taskId, time));
            }
            return reminders.Where(n => n.Reminder.Year > 2019).ToArray();
        }

        private async Task<(string FileUrl, string FileName)[]> GetListFiles(int listId)
        {
            string responseContent = await _httpClient.GetStringAsync(Url + "files?list_id=" + listId);
            JArray jContent = JArray.Parse(responseContent);
            List<(string FileUrl, string FileName)> fileInfoList = new List<(string FileUrl, string FileName)>();
            foreach (JToken jFile in jContent)
            {
                long taskId = jFile["task_id"].Value<long>();
                string fileUrl = jFile["url"].Value<string>();
                string fileType = jFile["content_type"].Value<string>().Replace("/", "-");
                string fileName = Path.Combine(FilesDirectory,
                    $"cat={_categoryId}_task={taskId}_type={fileType}_{jFile["file_name"].Value<string>()}");
                fileInfoList.Add((fileUrl, fileName));
            }
            return fileInfoList.ToArray();
        }

        private void DownloadFiles((string FileUrl, string FileName)[] files)
        {
            foreach ((string FileUrl, string FileName) file in files)
            {
                if (File.Exists(file.FileName)) File.Delete(file.FileName);
                _webClient.DownloadFile(file.FileUrl, file.FileName);
            }
        }
    }
}

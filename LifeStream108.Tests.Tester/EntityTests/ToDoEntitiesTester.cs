using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.ToDoListManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class ToDoEntitiesTester
    {
        private const int Category = 1;
        private const string TaskName1 = "Task 1";
        private const string TaskName2 = "Task 1a";

        public static void Run()
        {
            DeleteTasks();

            ToDoCategory[] cats = ToDoCategoryManager.GetUserCategories(UserTester.UserId_1);
            Assert.IsTrue(cats.Length > 0, $"No todo categories for user {UserTester.UserId_1}");

            ToDoCategory[] cats2 = ToDoCategoryManager.GetUserCategories(UserTester.UserId_2);
            Assert.IsTrue(cats2.Length == 0, $"Why user {UserTester.UserId_2} has todo categories?");

            ToDoList[] lists = ToDoListManager.GetCategoryLists(Category);
            Assert.IsTrue(lists.Length > 0, $"No lists for category {Category}");

            ToDoList list = lists[0];

            ToDoTask newTask = new ToDoTask
            {
                UserId = UserTester.UserId_1,
                ListId = list.Id,
                Title = TaskName1
            };
            ToDoTaskManager.AddTask(newTask);
            Assert.IsTrue(newTask.Id > 0, "Task has no id");

            ToDoTask task = ToDoTaskManager.GetTask(newTask.Id);
            Assert.IsNotNull(task, "Get task by id not work");

            task = ToDoTaskManager.GetTaskByTitle(newTask.Title, list.Id);
            Assert.IsNotNull(task, "Get task by title not work");

            task.Title = TaskName2;
            task.ReminderSettings = "Reminder";
            task.Status = ToDoTaskStatus.Completed;
            ToDoTaskManager.UpdateTask(task);
            ToDoTask updatedTask = ToDoTaskManager.GetTask(task.Id);
            Assert.IsTrue(
                updatedTask.Title == task.Title
                && updatedTask.ReminderSettings == task.ReminderSettings
                && updatedTask.Status == task.Status,
                "Update task not work");

            ToDoTask[] foundTasks = ToDoTaskManager.FindTasks("task", 100, UserTester.UserId_1);
            Assert.IsTrue(foundTasks.Length > 0, "Find tasks not work");

            ToDoTask[] listTasks = ToDoTaskManager.GetListActiveTasks(list.Id);
            Assert.IsTrue(listTasks.Length > 0, $"List {list.Id} has no tasks");

            DeleteTasks();
        }

        private static void DeleteTasks()
        {
            string tableName = TestHelper.GetClassConstantValue("TableName", typeof(ToDoTaskManager));
            PostgreSqlCommandUtils.UpdateEntity(
                $"delete from {tableName} where title in ('{TaskName1}', '{TaskName2}')", null);
        }
    }
}

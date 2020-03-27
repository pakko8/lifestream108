using LifeStream108.Libs.Entities.ToDoEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.ToDoListManagement.Managers
{
    public static class ToDoTaskManager
    {
        public static ToDoTask GetTask(int taskId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return CommonManager<ToDoTask>.GetById(taskId, session);
            }
        }

        public static ToDoTask GetTaskByTitle(string title, int listId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from task in session.Query<ToDoTask>()
                            where task.Title == title.Trim() && task.ListId == listId && task.Status != ToDoTaskStatus.Deleted
                            select task;
                return query.FirstOrDefault();
            }
        }

        public static ToDoTask[] GetListActiveTasks(int listId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from task in session.Query<ToDoTask>()
                            where task.ListId == listId && task.Status != ToDoTaskStatus.Deleted
                            select task;
                return query.ToArray();
            }
        }

        public static ToDoTask[] FindTasks(string word, int limit, int userId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from task in session.Query<ToDoTask>()
                            where task.UserId == userId && task.Status != ToDoTaskStatus.Deleted && task.Title.ToUpper().Contains(word.ToUpper())
                            select task;
                return query.Take(limit).ToArray();
            }
        }

        public static void AddTask(ToDoTask item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<ToDoTask>.Add(item, session);
                session.Flush();
            }
        }

        public static void UpdateTask(ToDoTask item)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<ToDoTask>.Update(item, session);
                session.Flush();
            }
        }
    }
}

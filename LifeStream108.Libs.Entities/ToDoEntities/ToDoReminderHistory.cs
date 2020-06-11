using System;

namespace LifeStream108.Libs.Entities.ToDoEntities
{
    public class ToDoReminderHistory
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        public DateTime ReminderTime { get; set; } = DateTime.Now;
    }
}

using System;

namespace LifeStream108.Libs.Entities.ToDoEntities
{
    public class ToDoReminderHistory
    {
        public virtual int Id { get; set; }

        public virtual int TaskId { get; set; }

        public virtual DateTime ReminderTime { get; set; } = DateTime.Now;
    }
}

using System;

namespace LifeStream108.Libs.Entities.ToDoEntities
{
    public class ToDoTask
    {
        public virtual int Id { get; set; }

        public virtual int SortOrder { get; set; } = 1;

        public virtual int UserCode { get; set; }

        public virtual int ListId { get; set; }

        public virtual int UserId { get; set; }

        public virtual string Title { get; set; }

        public virtual string Note { get; set; } = "";

        public virtual string Files { get; set; } = "";

        public virtual bool Active { get; set; } = true;

        public virtual DateTime RegTime { get; set; } = DateTime.Now;

        public virtual string ReminderSettings { get; set; } = "";

        public virtual DateTime ReminderLastTime { get; set; } = new DateTime(2000, 1, 1);
    }
}

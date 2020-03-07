using System;

namespace LifeStream108.Libs.Entities.ToDoEntities
{
    public class ToDoList
    {
        public virtual int Id { get; set; }

        public virtual int SortOrder { get; set; } = 1;

        public virtual int UserCode { get; set; }

        public virtual int CategoryId { get; set; }

        public virtual int UserId { get; set; }

        public virtual string Name { get; set; }

        public virtual bool Active { get; set; } = true;

        public virtual DateTime RegTime { get; set; } = DateTime.Now;
    }
}

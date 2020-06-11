using System;

namespace LifeStream108.Libs.Entities.ToDoEntities
{
    public class ToDoList
    {
        public int Id { get; set; }

        public int SortOrder { get; set; } = 1;

        public int UserCode { get; set; }

        public int CategoryId { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; } = true;

        public DateTime RegTime { get; set; } = DateTime.Now;
    }
}

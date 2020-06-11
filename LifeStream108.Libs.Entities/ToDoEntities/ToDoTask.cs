using System;

namespace LifeStream108.Libs.Entities.ToDoEntities
{
    public class ToDoTask
    {
        public int Id { get; set; }

        public int SortOrder { get; set; } = 1;

        public int ListId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; } = "";

        public string Files { get; set; } = "";

        public ToDoTaskStatus Status { get; set; } = ToDoTaskStatus.New;

        public DateTime RegTime { get; set; } = DateTime.Now;

        public DateTime ContentUpdateTime { get; set; } = DateTime.Now;

        public DateTime StatusUpdateTime { get; set; } = new DateTime(2000, 1, 1);

        public string ReminderSettings { get; set; } = "";

        public bool IsRepetitive { get; set; } = false;

        public DateTime ReminderLastTime { get; set; } = new DateTime(2000, 1, 1);

        public override bool Equals(object obj)
        {
            return ((ToDoTask)obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }
    }
}

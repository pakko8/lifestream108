using System;

namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public class LifeActivityLog
    {
        public long Id { get; set; }

        public int UserId { get; set; }

        public int LifeActivityId { get; set; }

        public DateTime Period { get; set; }

        public bool Active { get; set; } = true;

        public string Comment { get; set; }

        public DateTime RegTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}

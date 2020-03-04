using System;

namespace LifeStream108.Libs.Entities
{
    public class LifeActivityLog
    {
        public virtual long Id { get; set; }

        public virtual int UserId { get; set; }

        public virtual int LifeActivityId { get; set; }

        public virtual DateTime Period { get; set; }

        public virtual bool Active { get; set; } = true;

        public virtual string Comment { get; set; }

        public virtual DateTime RegTime { get; set; } = DateTime.Now;

        public virtual DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}

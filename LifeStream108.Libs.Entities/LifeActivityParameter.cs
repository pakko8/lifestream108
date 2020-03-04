using System;

namespace LifeStream108.Libs.Entities
{
    public class LifeActivityParameter
    {
        public virtual int Id { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual int UserCode { get; set; }

        public virtual int UserId { get; set; }

        public virtual int ActivityId { get; set; }

        public virtual string Name { get; set; }

        public virtual string NameForUser => $"[{UserCode}] {Name}";

        public virtual int MeasureId { get; set; }

        public virtual DataType DataType { get; set; }

        public virtual string Fuction { get; set; } = "";

        public virtual bool Active { get; set; } = true;

        public virtual DateTime RegTime { get; set; } = DateTime.Now;
    }
}

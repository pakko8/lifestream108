using System;

namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public class LifeGroup
    {
        public virtual int Id { get; set; }

        public virtual int UserCode { get; set; }

        public virtual int UserId { get; set; }

        public virtual string Name { get; set; }

        public virtual string NameForUser => $"[{UserCode}] {Name}";

        public virtual string ShortName { get; set; } = "";

        public virtual bool Active { get; set; } = true;

        public virtual DateTime RegTime { get; set; } = DateTime.Now;
    }
}

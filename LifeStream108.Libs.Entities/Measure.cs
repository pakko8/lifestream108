using System;

namespace LifeStream108.Libs.Entities
{
    public class Measure
    {
        public virtual int Id { get; set; }

        public virtual int UserCode { get; set; }

        public virtual int UserId { get; set; }

        public virtual string Name { get; set; }

        public virtual string ShortName { get; set; } = "";

        public virtual string Declanation1 { get; set; } = "";

        public virtual string Declanation2 { get; set; } = "";

        public virtual string Declanation3 { get; set; } = "";

        public virtual DateTime RegTime { get; set; } = DateTime.Now;
    }
}

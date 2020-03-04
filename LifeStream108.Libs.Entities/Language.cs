using System;

namespace LifeStream108.Libs.Entities
{
    public class Language
    {
        public virtual int Id { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual string Name { get; set; }

        public virtual string ShortName { get; set; }

        public virtual DateTime RegTime { get; set; }
    }
}

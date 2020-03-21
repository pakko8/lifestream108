using System;

namespace LifeStream108.Libs.Entities.CommandEntities
{
    public class Command
    {
        public virtual int Id { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual EntityType EntityType { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string ProcessorClassName { get; set; }

        public virtual bool Active { get; set; }

        public virtual DateTime RegTime { get; set; }
    }
}

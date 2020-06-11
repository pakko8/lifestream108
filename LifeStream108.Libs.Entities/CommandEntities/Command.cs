using System;

namespace LifeStream108.Libs.Entities.CommandEntities
{
    public class Command
    {
        public int Id { get; set; }

        public int SortOrder { get; set; }

        public int ProjectId { get; set; }

        public EntityType EntityType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProcessorClassName { get; set; }

        public bool Active { get; set; }
    }
}

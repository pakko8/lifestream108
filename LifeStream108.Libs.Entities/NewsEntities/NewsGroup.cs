using System;

namespace LifeStream108.Libs.Entities.NewsEntities
{
    public class NewsGroup
    {
        public virtual int Id { get; set; }

        public virtual int Priority { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string Url { get; set; }

        public virtual string ProcessorClassName { get; set; }

        public virtual bool Active { get; set; }

        public virtual DateTime RegTime { get; set; }

        public virtual int CheckIntervalInMinutes { get; set; }

        public virtual DateTime LastRunTime { get; set; }

        public virtual NewsGroupRunStatus RunStatus { get; set; }
    }
}

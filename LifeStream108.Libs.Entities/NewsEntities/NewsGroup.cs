using System;

namespace LifeStream108.Libs.Entities.NewsEntities
{
    public class NewsGroup
    {
        public int Id { get; set; }

        public int Priority { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string ProcessorClassName { get; set; }

        public bool Active { get; set; }

        public DateTime RegTime { get; set; }

        public int CheckIntervalInMinutes { get; set; }

        public DateTime LastRunTime { get; set; }

        public NewsGroupRunStatus RunStatus { get; set; }
    }
}

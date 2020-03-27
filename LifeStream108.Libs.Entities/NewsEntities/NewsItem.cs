using System;

namespace LifeStream108.Libs.Entities.NewsEntities
{
    public class NewsItem
    {
        public virtual int Id { get; set; }

        public virtual int NewsGroupId { get; set; }

        public virtual string Title { get; set; }

        public virtual string Url { get; set; }

        public virtual string ResourceId { get; set; }

        public virtual DateTime NewsTime { get; set; }

        public virtual DateTime RegTime { get; set; } = DateTime.Now;
    }
}

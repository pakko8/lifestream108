using System;

namespace LifeStream108.Libs.Entities.NewsEntities
{
    public class NewsHistoryItem
    {
        public int Id { get; set; }

        public int NewsGroupId { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string ResourceId { get; set; }

        public DateTime NewsTime { get; set; }

        public DateTime RegTime { get; set; } = DateTime.Now;
    }
}

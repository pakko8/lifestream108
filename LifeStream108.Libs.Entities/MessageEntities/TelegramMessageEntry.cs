using System;

namespace LifeStream108.Libs.Entities.MessageEntities
{
    public class TelegramMessageEntry
    {
        public int Id { get; set; }

        public int TelegramUserId { get; set; }

        public long ChatId { get; set; }

        public int MessageId { get; set; }

        public DateTime RegTime { get; set; } = DateTime.Now;
    }
}

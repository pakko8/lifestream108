using System;

namespace LifeStream108.Libs.Entities.MessageEntities
{
    public class TelegramMessageEntry
    {
        public virtual int Id { get; set; }

        public virtual int TelegramUserId { get; set; }

        public virtual long ChatId { get; set; }

        public virtual int MessageId { get; set; }

        public virtual DateTime RegTime { get; set; } = DateTime.Now;
    }
}

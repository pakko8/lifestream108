using System;

namespace LifeStream108.Libs.Entities.TicketEntities
{
    public class BugTicket
    {
        public virtual int Id { get; set; }

        public virtual string ErrorType { get; set; }

        public virtual int UserId { get; set; }

        public virtual string RequestDetails { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual DateTime RegTime { get; set; } = DateTime.Now;

        public virtual DateTime FixTime { get; set; } = new DateTime(2000, 1, 1);

        public virtual DateTime NotificationSentTime { get; set; } = new DateTime(2000, 1, 1);

        public virtual string MessageForUser { get; set; } = "";

        public virtual BugTicketStatus Status { get; set; } = BugTicketStatus.New;
    }
}
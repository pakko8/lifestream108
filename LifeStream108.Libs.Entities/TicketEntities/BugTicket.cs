using System;

namespace LifeStream108.Libs.Entities.TicketEntities
{
    public class BugTicket
    {
        public int Id { get; set; }

        public string ErrorType { get; set; }

        public int UserId { get; set; }

        public string RequestDetails { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime RegTime { get; set; } = DateTime.Now;

        public DateTime FixTime { get; set; } = new DateTime(2000, 1, 1);

        public DateTime NotificationSentTime { get; set; } = new DateTime(2000, 1, 1);

        public string MessageForUser { get; set; } = "";

        public BugTicketStatus Status { get; set; } = BugTicketStatus.New;
    }
}
using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.TicketEntities;

namespace LifeStream108.Modules.TempDataManagement.Mapping
{
    public class BugTicketMap : ClassMap<BugTicket>
    {
        private const string TableName = "bug_tickets";

        public BugTicketMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.ErrorType, "error_type").Not.Nullable();
            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.RequestDetails, "request_details").Not.Nullable();
            Map(x => x.ErrorMessage, "error_message").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
            Map(x => x.FixTime, "fix_time").Not.Nullable();
            Map(x => x.NotificationSentTime, "notification_sent_time").Not.Nullable();
            Map(x => x.MessageForUser, "message_for_user").Not.Nullable();
            Map(x => x.Status, "status").CustomType<BugTicketStatus>().Not.Nullable();
        }
    }
}

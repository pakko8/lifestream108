using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.MessageEntities;

namespace LifeStream108.Modules.TempDataManagement.Mapping
{
    public class TelegramMessageEntryMap : ClassMap<TelegramMessageEntry>
    {
        private const string TableName = "telegram_msg_entries";

        public TelegramMessageEntryMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.TelegramUserId, "tlgrm_user_id").Not.Nullable();
            Map(x => x.ChatId, "chat_id").Not.Nullable();
            Map(x => x.MessageId, "message_id").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

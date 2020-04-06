using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.ToDoEntities;

namespace LifeStream108.Modules.ToDoListManagement.Mapping
{
    public class ToDoTaskMap : ClassMap<ToDoTask>
    {
        private const string TableName = "todo_tasks";

        public ToDoTaskMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.SortOrder, "sort_order").Not.Nullable();
            Map(x => x.ListId, "list_id").Not.Nullable();
            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.Title, "title").Not.Nullable();
            Map(x => x.Note, "note").Not.Nullable();
            Map(x => x.Files, "files").Not.Nullable();
            Map(x => x.Status, "status").CustomType<ToDoTaskStatus>().Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
            Map(x => x.ContentUpdateTime, "content_update_time").Not.Nullable();
            Map(x => x.StatusUpdateTime, "status_update_time").Not.Nullable();
            Map(x => x.ReminderSettings, "reminder_sett").Not.Nullable();
            Map(x => x.IsRepetitive, "repetitive").Not.Nullable();
            Map(x => x.ReminderLastTime, "reminder_last_time").Not.Nullable();
        }
    }
}

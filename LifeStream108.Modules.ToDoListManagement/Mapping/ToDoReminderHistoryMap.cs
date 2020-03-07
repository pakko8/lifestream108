using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.ToDoEntities;

namespace LifeStream108.Modules.ToDoListManagement.Mapping
{
    public class ToDoReminderHistoryMap : ClassMap<ToDoReminderHistory>
    {
        private const string TableName = "todo_reminder_history";

        public ToDoReminderHistoryMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.TaskId, "task_id").Not.Nullable();
            Map(x => x.ReminderTime, "reg_time").Not.Nullable();
        }
    }
}

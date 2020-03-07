using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.ToDoEntities;

namespace LifeStream108.Modules.ToDoListManagement.Mapping
{
    public class ToDoListMap : ClassMap<ToDoList>
    {
        private const string TableName = "todo_lists";

        public ToDoListMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.SortOrder, "sort_order").Not.Nullable();
            Map(x => x.UserCode, "user_code").Not.Nullable();
            Map(x => x.CategoryId, "category_id").Not.Nullable();
            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.Active, "active").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

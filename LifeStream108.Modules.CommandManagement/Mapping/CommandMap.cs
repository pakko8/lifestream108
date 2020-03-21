using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.CommandEntities;

namespace LifeStream108.Modules.CommandManagement.Mapping
{
    public class CommandMap : ClassMap<Command>
    {
        private const string TableName = "commands";

        public CommandMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.SortOrder, "sort_order").Not.Nullable();
            Map(x => x.EntityType, "entity_type").CustomType<EntityType>().Not.Nullable();
            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.Description, "description").Not.Nullable();
            Map(x => x.ProcessorClassName, "processor_class_name").Not.Nullable();
            Map(x => x.Active, "active").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.CommandEntities;

namespace LifeStream108.Modules.CommandManagement.Mapping
{
    public class CommandNameMap : ClassMap<CommandName>
    {
        private const string TableName = "command_names";

        public CommandNameMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.SortOrder, "sort_order").Not.Nullable();
            Map(x => x.CommandId, "command_id").Not.Nullable();
            Map(x => x.Alias, "alias").Not.Nullable();
            Map(x => x.SpacePositions, "space_positions").Not.Nullable();
            Map(x => x.LanguageId, "language_id").Not.Nullable();
        }
    }
}

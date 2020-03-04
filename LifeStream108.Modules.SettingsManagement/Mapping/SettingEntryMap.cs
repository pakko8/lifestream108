using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.SettingsManagement.Mapping
{
    public class SettingEntryMap : ClassMap<SettingEntry>
    {
        private const string TableName = "settings";

        public SettingEntryMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.SortOrder, "sort_order").Not.Nullable();
            Map(x => x.Code, "code").Not.Nullable();
            Map(x => x.Description, "description").Not.Nullable();
            Map(x => x.DataType, "data_type").Not.Nullable();
            Map(x => x.Value, "value").Not.Nullable();
            Map(x => x.Options, "options").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

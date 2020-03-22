using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.DictionaryEntities;

namespace LifeStream108.Modules.DictionaryManagement.Mapping
{
    public class ProjectSettingEntryMap : ClassMap<Project>
    {
        private const string TableName = "projects";

        public ProjectSettingEntryMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.UserCode, "user_code").Not.Nullable();
            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.Description, "description").Not.Nullable();
            Map(x => x.HelpUrl, "help_url").Not.Nullable();
            Map(x => x.AssemblyName, "assembly_name").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

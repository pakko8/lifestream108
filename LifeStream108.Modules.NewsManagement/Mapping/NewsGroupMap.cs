using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.NewsEntities;

namespace LifeStream108.Modules.NewsManagement.Mapping
{
    public class NewsGroupMap : ClassMap<NewsGroup>
    {
        private const string TableName = "news_groups";

        public NewsGroupMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.Priority, "priority").Not.Nullable();
            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.Description, "description").Not.Nullable();
            Map(x => x.Url, "url").Not.Nullable();
            Map(x => x.ProcessorClassName, "processor_class_name").Not.Nullable();
            Map(x => x.Active, "active").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
            Map(x => x.CheckIntervalInMinutes, "check_interval_in_minutes").Not.Nullable();
            Map(x => x.LastRunTime, "last_run_time").Not.Nullable();
            Map(x => x.RunStatus, "run_status").Not.Nullable();
        }
    }
}

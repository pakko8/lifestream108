using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.NewsManagement.Mapping
{
    public class NewsItemMap : ClassMap<NewsItem>
    {
        private const string TableName = "news_items";
        public NewsItemMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.NewsGroupId, "news_group_id").Not.Nullable();
            Map(x => x.Title, "title").Not.Nullable();
            Map(x => x.Url, "url").Not.Nullable();
            Map(x => x.ResourceId, "resource_id").Not.Nullable();
            Map(x => x.NewsTime, "news_time").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.DictionaryEntities;

namespace LifeStream108.Modules.DictionaryManagement.Mapping
{
    public class MeasureMap : ClassMap<Measure>
    {
        private const string TableName = "measures";

        public MeasureMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.UserCode, "user_code").Not.Nullable();
            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.ShortName, "short_name").Not.Nullable();
            Map(x => x.Declanation1, "declanation1").Not.Nullable();
            Map(x => x.Declanation2, "declanation2").Not.Nullable();
            Map(x => x.Declanation3, "declanation3").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

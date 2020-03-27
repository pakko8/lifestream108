using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.LifeActityEntities;

namespace LifeStream108.Modules.LifeActivityManagement.Mapping
{
    public class LifeActivityMap : ClassMap<LifeActivity>
    {
        private const string TableName = "life_activities";

        public LifeActivityMap()
        {
            Schema(Constants.ActivitiesSchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.UserCode, "user_code").Not.Nullable();
            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.LifeGroupAtGroupId, "life_group_at_group_id").Not.Nullable();
            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.ShortName, "short_name").Not.Nullable();
            Map(x => x.Active, "active").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

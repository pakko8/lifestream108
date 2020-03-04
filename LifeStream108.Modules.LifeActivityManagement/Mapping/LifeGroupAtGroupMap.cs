using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.LifeActivityManagement.Mapping
{
    public class LifeGroupAtGroupMap : ClassMap<LifeGroupAtGroup>
    {
        private const string TableName = "life_groups_at_groups";

        public LifeGroupAtGroupMap()
        {
            Schema(Constants.ActivitiesSchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.LifeGroupId, "life_group_id").Not.Nullable();
            Map(x => x.ParentLifeGroupId, "parent_life_group_id").Not.Nullable();
            Map(x => x.SortOrder, "sort_order").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

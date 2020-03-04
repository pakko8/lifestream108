using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.LifeActivityManagement.Mapping
{
    public class LifeActivityParameterMap : ClassMap<LifeActivityParameter>
    {
        private const string TableName = "life_activity_params";

        public LifeActivityParameterMap()
        {
            Schema(Constants.ActivitiesSchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.SortOrder, "sort_order").Not.Nullable();
            Map(x => x.UserCode, "user_code").Not.Nullable();
            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.ActivityId, "activity_id").Not.Nullable();
            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.MeasureId, "measure_id").Not.Nullable();
            Map(x => x.DataType, "data_type").Not.Nullable();
            Map(x => x.Fuction, "func").Not.Nullable();
            Map(x => x.Active, "active").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

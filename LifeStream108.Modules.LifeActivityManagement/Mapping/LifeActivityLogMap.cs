using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.LifeActivityManagement.Mapping
{
    public class LifeActivityLogMap : ClassMap<LifeActivityLog>
    {
        private const string TableName = "life_activity_logs";

        public LifeActivityLogMap()
        {
            Schema(Constants.LogsSchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.LifeActivityId, "life_activity_id").Not.Nullable();
            Map(x => x.Period, "period").Not.Nullable();
            Map(x => x.Active, "active").Not.Nullable();
            Map(x => x.Comment, "comment").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
            Map(x => x.UpdateTime, "update_time").Not.Nullable();
        }
    }
}

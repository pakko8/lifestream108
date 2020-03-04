using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.LifeActivityManagement.Mapping
{
    public class LifeActivityLogValueMap : ClassMap<LifeActivityLogValue>
    {
        private const string TableName = "life_activity_log_values";

        public LifeActivityLogValueMap()
        {
            Schema(Constants.LogsSchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.ActivityLogId, "activity_log_id").Not.Nullable();
            Map(x => x.Period, "period").Not.Nullable();
            Map(x => x.ActivityParamId, "activity_param_id").Not.Nullable();
            Map(x => x.NumericValue, "numeric_value").Not.Nullable();
            Map(x => x.TextValue, "text_value");
        }
    }
}

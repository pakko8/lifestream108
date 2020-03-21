using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.LifeActityEntities;

namespace LifeStream108.Modules.LifeActivityManagement.Mapping
{
    public class LifeActivityPlanMap : ClassMap<LifeActivityPlan>
    {
        private const string TableName = "life_activity_plans";

        public LifeActivityPlanMap()
        {
            Schema(Constants.ActivitiesSchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.LifeActivityId, "life_activity_id").Not.Nullable();
            Map(x => x.Year, "year").Not.Nullable();
            Map(x => x.Month, "month").Not.Nullable();
            Map(x => x.MinPlanValue, "min_plan_value").Not.Nullable();
            Map(x => x.MiddlePlanValue, "middle_plan_value").Not.Nullable();
            Map(x => x.MaxPlanValue, "max_plan_value").Not.Nullable();
            Map(x => x.NotifyTimeMinPlan, "notify_time_min_plan").Not.Nullable();
            Map(x => x.NotifyTimeMiddlePlan, "notify_time_middle_plan").Not.Nullable();
            Map(x => x.NotifyTimeMaxPlan, "notify_time_max_plan").Not.Nullable();
            Map(x => x.NotifyTimeResume, "notify_time_resume").Not.Nullable();
            Map(x => x.Active, "active").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

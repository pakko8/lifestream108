using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities.SessionEntities;

namespace LifeStream108.Modules.TempDataManagement.Mapping
{
    public class SessionMap : ClassMap<Session>
    {
        private const string TableName = "sessions";

        public SessionMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.UserId, "user_id").Not.Nullable();
            Map(x => x.ProjectId, "project_id").Not.Nullable();
            Map(x => x.LastCommandId, "last_command_id").Not.Nullable();
            Map(x => x.LastLifeGroupId, "last_life_group_id").Not.Nullable();
            Map(x => x.LastLifeActivityId, "last_life_activity_id").Not.Nullable();
            Map(x => x.LastRequestText, "last_request_text").Not.Nullable();
            Map(x => x.Data, "data").Not.Nullable();
            Map(x => x.StartTime, "start_time").Not.Nullable();
            Map(x => x.LastActivityTime, "last_activity_time").Not.Nullable();
        }
    }
}

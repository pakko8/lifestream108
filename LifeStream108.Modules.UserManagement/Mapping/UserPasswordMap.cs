using FluentNHibernate.Mapping;

namespace LifeStream108.Modules.UserManagement.Mapping
{
    internal class UserPasswordMap : ClassMap<UserAuth>
    {
        private const string TableName = "users";

        public UserPasswordMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.UserId, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.Email, "email").Not.Nullable();
            Map(x => x.PasswordHash, "password_hash").Not.Nullable();
        }
    }
}

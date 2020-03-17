using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.UserManagement.Mapping
{
    public class UserMap : ClassMap<User>
    {
        private const string TableName = "users";

        public UserMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.Email, "email").Not.Nullable();
            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.TelegramId, "telegram_id").Not.Nullable();
            Map(x => x.LanguageId, "language_id").Not.Nullable();
            Map(x => x.CurrencyId, "currency_id").Not.Nullable();
            Map(x => x.Superuser, "superuser").Not.Nullable();
            Map(x => x.Status, "status").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.DictionaryManagement.Mapping
{
    public class CurrencyMap : ClassMap<Currency>
    {
        private const string TableName = "currencies";

        public CurrencyMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.LetterCode, "letter_code").Not.Nullable();
            Map(x => x.SortOrder, "sort_order").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

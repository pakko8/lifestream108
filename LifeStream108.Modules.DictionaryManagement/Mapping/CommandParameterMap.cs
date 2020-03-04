using FluentNHibernate.Mapping;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.DictionaryManagement.Mapping
{
    public class CommandParameterMap : ClassMap<CommandParameter>
    {
        private const string TableName = "command_params";

        public CommandParameterMap()
        {
            Schema(Constants.SchemaName);
            Table(TableName);
            Id(x => x.Id, "id").GeneratedBy.Sequence(TableName + "_id_seq");

            Map(x => x.CommandId, "command_id").Not.Nullable();
            Map(x => x.SortOrder, "sort_order").Not.Nullable();
            Map(x => x.ParameterCode, "command_param_code").Not.Nullable();
            Map(x => x.Name, "name").Not.Nullable();
            Map(x => x.Description, "description").Not.Nullable();
            Map(x => x.DataType, "data_type").Not.Nullable();
            Map(x => x.DataFormat, "data_format").Not.Nullable();
            Map(x => x.InputDataType, "input_data_type").Not.Nullable();
            Map(x => x.DataFormatDescription, "data_format_desc").Not.Nullable();
            Map(x => x.Required, "required").Not.Nullable();
            Map(x => x.MinLength, "min_length").Not.Nullable();
            Map(x => x.MaxLength, "max_length").Not.Nullable();
            Map(x => x.Regex, "regex").Not.Nullable();
            Map(x => x.PredefinedValues, "predefined_values").Not.Nullable();
            Map(x => x.DefaultValue, "default_value").Not.Nullable();
            Map(x => x.Example, "example").Not.Nullable();
            Map(x => x.RegTime, "reg_time").Not.Nullable();
        }
    }
}

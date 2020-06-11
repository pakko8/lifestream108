using System;
using System.Linq;

namespace LifeStream108.Libs.Entities.CommandEntities
{
    public class CommandParameter
    {
        public int Id { get; set; }

        public int CommandId { get; set; }

        public int SortOrder { get; set; }

        public CommandParameterCode ParameterCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DataType DataType { get; set; }

        public InputDataType InputDataType { get; set; }

        public string DataFormat { get; set; }

        public string DataFormatDescription { get; set; }

        public bool Required { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }

        public string Regex { get; set; }

        public string PredefinedValues { get; set; }

        public string[] PredefinedValuesList =>
            PredefinedValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(n => n.ToUpper()).ToArray();

        public string DefaultValue { get; set; }

        public string Example { get; set; }
    }
}

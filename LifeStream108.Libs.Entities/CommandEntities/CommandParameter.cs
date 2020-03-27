using System;
using System.Linq;

namespace LifeStream108.Libs.Entities.CommandEntities
{
    public class CommandParameter
    {
        public virtual int Id { get; set; }

        public virtual int CommandId { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual CommandParameterCode ParameterCode { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual DataType DataType { get; set; }

        public virtual InputDataType InputDataType { get; set; }

        public virtual string DataFormat { get; set; }

        public virtual string DataFormatDescription { get; set; }

        public virtual bool Required { get; set; }

        public virtual int MinLength { get; set; }

        public virtual int MaxLength { get; set; }

        public virtual string Regex { get; set; }

        public virtual string PredefinedValues { get; set; }

        public virtual string[] PredefinedValuesList =>
            PredefinedValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(n => n.ToUpper()).ToArray();

        public virtual string DefaultValue { get; set; }

        public virtual string Example { get; set; }
    }
}

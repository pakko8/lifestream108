namespace LifeStream108.Libs.Entities
{
    public enum DataType
    {
        Text,

        Integer,

        Double,

        Date,

        Period
    }

    public static class DataTypeExtensions
    {
        public static string GetDescriptiveName(this DataType dataType)
        {
            switch(dataType)
            {
                case DataType.Text: return "Текст";
                case DataType.Integer:
                case DataType.Double: return "Число";
                case DataType.Date: return "Дата";
                case DataType.Period: return "Период дат";
                default: return "Неизвестно";
            }
        }
    }
}

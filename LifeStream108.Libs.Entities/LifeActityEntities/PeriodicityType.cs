namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public enum PeriodicityType
    {
        None = 0,

        Daily = 1,

        Monthly = 2,

        Annual = 3
    }

    public static class PeriodicityTypeExtensions
    {
        public static string GetDescriptiveName(this PeriodicityType type)
        {
            switch (type)
            {
                case PeriodicityType.Daily: return "Ежедневный";
                case PeriodicityType.Monthly: return "Ежемесячный";
                case PeriodicityType.Annual: return "Ежегодный";
                case PeriodicityType.None: return "Обычный";
                default: return type.ToString();
            }
        }
    }
}

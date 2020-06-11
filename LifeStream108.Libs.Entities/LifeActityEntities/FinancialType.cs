namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public enum FinancialType
    {
        None = 0,

        Income = 1,

        Expenses = 2
    }

    public static class FinancialTypeExtensions
    {
        public static string GetDescriptiveName(this FinancialType type)
        {
            switch (type)
            {
                case FinancialType.Income: return "Доходы";
                case FinancialType.Expenses: return "Расходы";
                case FinancialType.None: return "Обычный";
                default: return type.ToString();
            }
        }
    }
}

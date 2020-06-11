using System;

namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public class LifeGroup
    {
        public int Id { get; set; }

        public int UserCode { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string NameForUser => $"[{UserCode}] {Name}";

        public string ShortName { get; set; } = "";

        public FinancialType FinanceType { get; set; } = FinancialType.None;

        public bool Active { get; set; } = true;

        public DateTime RegTime { get; set; } = DateTime.Now;
    }
}

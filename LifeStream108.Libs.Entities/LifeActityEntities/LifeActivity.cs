using System;

namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public class LifeActivity
    {
        public int Id { get; set; }

        public int UserCode { get; set; }

        public int UserId { get; set; }

        public int LifeGroupAtGroupId { get; set; } = 0;

        public string Name { get; set; }

        public string NameForUser => $"[{UserCode}] {Name}";

        public string ShortName { get; set; } = "";

        public PeriodicityType PeriodType { get; set; } = PeriodicityType.None;

        public bool Active { get; set; } = true;

        public DateTime RegTime { get; set; } = DateTime.Now;
    }
}

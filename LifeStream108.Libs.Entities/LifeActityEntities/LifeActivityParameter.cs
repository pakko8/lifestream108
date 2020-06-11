using System;

namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public class LifeActivityParameter
    {
        public int Id { get; set; }

        public int SortOrder { get; set; }

        public int UserCode { get; set; }

        public int UserId { get; set; }

        public int ActivityId { get; set; }

        public string Name { get; set; }

        public string NameForUser => $"[{UserCode}] {Name}";

        public int MeasureId { get; set; }

        public DataType DataType { get; set; }

        public string Fuction { get; set; } = "";

        public bool Active { get; set; } = true;

        public DateTime RegTime { get; set; } = DateTime.Now;
    }
}

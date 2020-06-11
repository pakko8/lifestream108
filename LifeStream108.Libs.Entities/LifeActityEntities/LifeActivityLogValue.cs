using System;

namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public class LifeActivityLogValue
    {
        public long Id { get; set; }

        public int UserId { get; set; }

        public long ActivityLogId { get; set; }

        public DateTime Period { get; set; }

        public int ActivityParamId { get; set; }

        public double NumericValue { get; set; } = 0;

        public string TextValue { get; set; } = "";
    }
}

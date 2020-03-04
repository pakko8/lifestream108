using System;

namespace LifeStream108.Libs.Entities
{
    public class LifeActivityLogValue
    {
        public virtual long Id { get; set; }

        public virtual int UserId { get; set; }

        public virtual long ActivityLogId { get; set; }

        public virtual DateTime Period { get; set; }

        public virtual int ActivityParamId { get; set; }

        public virtual double NumericValue { get; set; } = 0;

        public virtual string TextValue { get; set; } = "";
    }
}

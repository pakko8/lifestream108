using System;

namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public class LifeGroupAtGroup
    {
        public virtual int Id { get; set; }

        public virtual int UserId { get; set; }

        public virtual int LifeGroupId { get; set; }

        public virtual int ParentLifeGroupId { get; set; }

        public virtual int SortOrder { get; set; } = 1;

        public virtual DateTime RegTime { get; set; } = DateTime.Now;
    }
}

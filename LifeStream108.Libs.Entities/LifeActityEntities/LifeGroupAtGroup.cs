using System;

namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public class LifeGroupAtGroup
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int LifeGroupId { get; set; }

        public int ParentLifeGroupId { get; set; }

        public int SortOrder { get; set; } = 1;

        public DateTime RegTime { get; set; } = DateTime.Now;
    }
}

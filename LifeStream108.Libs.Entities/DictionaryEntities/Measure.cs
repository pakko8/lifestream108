using System;

namespace LifeStream108.Libs.Entities.DictionaryEntities
{
    public class Measure
    {
        public int Id { get; set; }

        public int UserCode { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; } = "";

        public string Declanation1 { get; set; } = "";

        public string Declanation2 { get; set; } = "";

        public string Declanation3 { get; set; } = "";

        public DateTime RegTime { get; set; } = DateTime.Now;
    }
}

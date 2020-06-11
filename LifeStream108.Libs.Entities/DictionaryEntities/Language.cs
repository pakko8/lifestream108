using System;

namespace LifeStream108.Libs.Entities.DictionaryEntities
{
    public class Language
    {
        public int Id { get; set; }

        public int SortOrder { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public DateTime RegTime { get; set; }
    }
}

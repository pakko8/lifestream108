using System;

namespace LifeStream108.Libs.Entities.DictionaryEntities
{
    public class Project
    {
        public int Id { get; set; }

        public string UserCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Web page with help
        /// </summary>
        public string HelpUrl { get; set; }

        public string AssemblyName { get; set; }

        public string AssemblyRootNamespace { get; set; }

        public DateTime RegTime { get; set; }
    }
}

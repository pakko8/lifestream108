using System;

namespace LifeStream108.Libs.Entities.DictionaryEntities
{
    public class Project
    {
        public virtual int Id { get; set; }

        public virtual string UserCode { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        /// <summary>
        /// Web page with help
        /// </summary>
        public virtual string HelpUrl { get; set; }

        public virtual string AssemblyName { get; set; }

        public virtual DateTime RegTime { get; set; }
    }
}

using System.Configuration;
using System.Xml;

namespace LifeStream108.Libs.HibernateManagement
{
    /// <summary>
    /// Used in configuration declaration
    /// </summary>
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }
    }
}

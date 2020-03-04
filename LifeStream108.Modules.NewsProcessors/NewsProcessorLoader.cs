using System;
using System.Reflection;

namespace LifeStream108.Modules.NewsProcessors
{
    public class NewsProcessorLoader
    {
        public BaseNewsProcessor LoadClass(string className)
        {
            Assembly thisAssembly = Assembly.GetAssembly(GetType());
            Type handlerType = thisAssembly.GetType(thisAssembly.GetName().Name + "." + className, true, true);
            return (BaseNewsProcessor)Activator.CreateInstance(handlerType);
        }
    }
}

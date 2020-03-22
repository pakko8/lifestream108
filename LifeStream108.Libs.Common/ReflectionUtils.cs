using NLog;
using System;
using System.IO;
using System.Reflection;

namespace LifeStream108.Libs.Common
{
    public static class ReflectionUtils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static object LoadClass(string className, string assemblyName, string assemblyDirectory)
        {
            Logger.Info($"Loading class <{className}> from assembly <{assemblyName}> in directory <{assemblyDirectory}>");

            Assembly assembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, assemblyName));
            Type handlerType = assembly.GetType(className, true, true);
            return Activator.CreateInstance(handlerType);
        }
    }
}

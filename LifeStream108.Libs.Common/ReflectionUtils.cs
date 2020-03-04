using NLog;
using System;
using System.IO;
using System.Reflection;

namespace LifeStream108.Libs.Common
{
    public static class ReflectionUtils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static object LoadClassObject(string className, string fileName, string fileDirectory)
        {
            Logger.Info($"Loading class <{className}> from file <{fileName}> in directory <{fileDirectory}>");

            Assembly assembly = Assembly.LoadFrom(Path.Combine(fileDirectory, fileName));
            Type handlerType = assembly.GetType(className, true, true);
            return Activator.CreateInstance(handlerType);
        }
    }
}

using System;
using System.Linq;
using System.Reflection;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class TestHelper
    {
        public static string GetClassConstantValue(string propName, Type type)
        {
            FieldInfo[] constants = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo constantField = constants.FirstOrDefault(n => n.Name == propName);
            if (constantField == null) throw new Exception($"No constant with name  '{propName}' in class '{type}'");

            return constantField.GetValue(null).ToString();
        }
    }
}

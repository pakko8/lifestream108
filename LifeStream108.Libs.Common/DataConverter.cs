using System.ComponentModel;
using System.Globalization;

namespace LifeStream108.Libs.Common
{
    public static class DataConverter
    {
        public static T Parse<T>(string value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
        }
    }
}

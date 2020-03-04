using System.Globalization;

namespace LifeStream108.Libs.Common
{
    public static class DataFormatter
    {
        public static string FormatNumber(double value)
        {
            string format = value % 1 == 0 ? "# ### ##0" : "# ### ##0.00";
            return value.ToString(format, new CultureInfo("en-US")).Trim();
        }
    }
}

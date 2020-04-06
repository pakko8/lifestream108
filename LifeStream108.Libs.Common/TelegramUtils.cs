using System.Text.RegularExpressions;

namespace LifeStream108.Libs.Common
{
    public static class TelegramUtils
    {
        public static string RemoveUnsafeSigns(string str)
        {
            return Regex.Replace(str, "[<>]+", "");
        }
    }
}

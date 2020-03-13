using System.Web;

namespace LifeStream108.Web.Portal.App_Code
{
    public static class WebUtils
    {
        public static int GetRequestIntValue(string key, HttpRequest request, int defaultValue)
        {
            string requestValue = request[key];
            if (requestValue == null) return defaultValue;

            return int.Parse(requestValue);
        }
    }
}
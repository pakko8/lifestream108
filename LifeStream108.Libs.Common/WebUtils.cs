using System.IO;
using System.Net;

namespace LifeStream108.Libs.Common
{
    public static class WebUtils
    {
        public static string DownloadPage(string url)
        {
            WebClient client = new WebClient();
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string pageContent = reader.ReadToEnd();
            data.Close();
            reader.Close();

            return pageContent;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities;

namespace LifeStream108.Modules.NewsProcessors
{
    public class ViomsProcessor : BaseNewsProcessor
    {
        private const string SubDirName = "mailings";

        public override NewsItem[] GetLastNews(string url, int newsGroupId, DateTime fromTime)
        {
            string pageContent = WebUtils.LoadPage(url);
            bool allowParseData = false;
            string line;
            List<NewsItem> newsList = new List<NewsItem>();
            StringReader reader = new StringReader(pageContent);
            while ((line = reader.ReadLine()) != null)
            {
                if (!allowParseData && line.Contains("table-striped")) allowParseData = true;

                if (!allowParseData || (!line.StartsWith("<TD><A HREF=") && !line.Contains(SubDirName))) continue;


                string timeLine = reader.ReadLine();
                DateTime newsTime = RetrieveTime(timeLine);
                if (newsTime < fromTime) continue;

                NewsItem newsItem = new NewsItem();
                newsItem.NewsGroupId = newsGroupId;
                newsItem.Title = RetrieveTitle(line);
                newsItem.Url = RetrieveUrl(line, url);
                newsItem.ResourceId = RetrieveResourceId(newsItem.Url);
                newsItem.NewsTime = newsTime;
                newsList.Add(newsItem);
            }
            reader.Close();

            return newsList.ToArray();
        }

        private static string RetrieveTitle(string line)
        {
            line = line.Replace("<td>", "").Replace("</td>", "").Replace("</a>", "");
            int index = line.LastIndexOf('>');
            return line.Substring(index + 1);
        }

        private static string RetrieveUrl(string line, string url)
        {
            int index = url.LastIndexOf(".ru");
            string startUrl = url.Substring(0, index + ".ru".Length);

            int firstQuotIndex = line.IndexOf('"');
            int nextQuotIndex = line.IndexOf('"', firstQuotIndex + 1);
            return startUrl + line.Substring(firstQuotIndex + 1, nextQuotIndex - firstQuotIndex - 1);
        }

        private static string RetrieveResourceId(string newsItemUrl)
        {
            int index = newsItemUrl.LastIndexOf('/');
            return newsItemUrl.Substring(index + 1);
        }

        private static DateTime RetrieveTime(string line)
        {
            string newsTimeString = line.Replace("<td>", "").Replace("</td>", "");
            return DateTime.ParseExact(newsTimeString, "dd MMMM yyyy, HH:mm", new CultureInfo("ru-RU")).AddHours(3);

        }
    }
}

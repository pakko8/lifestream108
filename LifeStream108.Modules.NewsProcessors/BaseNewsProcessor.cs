using LifeStream108.Libs.Entities.NewsEntities;
using System;

namespace LifeStream108.Modules.NewsProcessors
{
    public abstract class BaseNewsProcessor
    {
        public abstract NewsItem[] GetLastNews(string url, int newsGroupId, DateTime fromTime);
    }
}

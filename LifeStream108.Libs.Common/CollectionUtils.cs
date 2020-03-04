using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeStream108.Libs.Common
{
    public static class CollectionUtils
    {
        public static string Array2String<T>(IEnumerable<T> items, string separator = ",", string beginFrame = "", string endFrame = "")
        {
            if (items == null || items.Count() == 0) return string.Empty;

            return string.Join(separator, items.Select(n => beginFrame + n + endFrame));
        }

        public static string Dictionary2String<T1, T2>(Dictionary<T1, T2> dictionary)
        {
            int i = 0;
            StringBuilder sbDictionary = new StringBuilder();
            foreach (KeyValuePair<T1, T2> kvp in dictionary)
            {
                sbDictionary.Append($"{kvp.Key}={kvp.Value}");
                if (i < dictionary.Count - 1) sbDictionary.Append(";");
            }
            return sbDictionary.ToString();
        }
    }
}

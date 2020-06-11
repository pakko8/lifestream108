using System;
using System.Collections.Specialized;

namespace LifeStream108.Libs.Entities.SessionEntities
{
    public class Session
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProjectId { get; set; }

        public int LastCommandId { get; set; }

        public int LastLifeGroupId { get; set; }

        public int LastLifeActivityId { get; set; }

        public string LastRequestText { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime LastActivityTime { get; set; }

        private string _data;
        private readonly StringDictionary _dataDictionary = new StringDictionary();

        public string Data
        {
            get { return _data ?? ""; }
            set
            {
                _data = value;
                PrepareDataDictionary();
            }
        }

        public void PrepareDataDictionary()
        {
            _dataDictionary.Clear();
            if (string.IsNullOrEmpty(_data)) return;

            string[] dataPairs = _data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string dataPair in dataPairs)
            {
                string[] keyAndValue = dataPair.Split('=');
                _dataDictionary.Add(keyAndValue[0].Trim(), keyAndValue[1].Trim());
            }
        }

        public (long Value, string Error) GetNumberDataValue(string key)
        {
            if (_dataDictionary.ContainsKey(key))
            {
                long numberValue = long.Parse(_dataDictionary[key]);
                return (numberValue, null);
            }
            return (0, $"Для кода \"{key}\" не найден объект");
        }
    }
}

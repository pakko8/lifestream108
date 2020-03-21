using System;
using System.Collections.Specialized;

namespace LifeStream108.Libs.Entities.SessionEntities
{
    public class Session
    {
        public virtual int Id { get; set; }

        public virtual int UserId { get; set; }

        public virtual ProjectType ProjectType { get; set; } = ProjectType.LifeActivity;

        public virtual int LastCommandId { get; set; }

        public virtual int LastLifeGroupId { get; set; }

        public virtual int LastLifeActivityId { get; set; }

        public virtual string LastRequestText { get; set; }

        public virtual DateTime StartTime { get; set; }

        public virtual DateTime LastActivityTime { get; set; }

        private string _data;
        private readonly StringDictionary _dataDictionary = new StringDictionary();

        public virtual string Data
        {
            get { return _data ?? ""; }
            set
            {
                _data = value;
                PrepareDataDictionary();
            }
        }

        public virtual void PrepareDataDictionary()
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

        public virtual (long Value, string Error) GetNumberDataValue(string key)
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

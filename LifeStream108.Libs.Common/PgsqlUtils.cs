using System;
using System.Data;

namespace LifeStream108.Libs.Common
{
    public static class PgsqlUtils
    {
        public static string GetString(string columnName, IDataReader reader, string defaultValue = "")
        {
            object objectValue = reader[columnName];
            return objectValue != null ? objectValue.ToString() : defaultValue;
        }

        public static bool GetBoolean(string columnName, IDataReader reader, bool defaultValue = false)
        {
            object objectValue = reader[columnName];
            return objectValue != null ? (bool)objectValue : defaultValue;
        }

        public static int GetInt(string columnName, IDataReader reader, int defaultValue = 0)
        {
            object objectValue = reader[columnName];
            return objectValue != null ? Convert.ToInt32(objectValue) : defaultValue;
        }

        public static long GetLong(string columnName, IDataReader reader, long defaultValue = 0)
        {
            object objectValue = reader[columnName];
            return objectValue != null ? Convert.ToInt64(objectValue) : defaultValue;
        }

        public static double GetDouble(string columnName, IDataReader reader, double defaultValue = 0)
        {
            object objectValue = reader[columnName];
            return objectValue != null && !(objectValue is DBNull) ? Convert.ToDouble(objectValue) : defaultValue;
        }

        public static DateTime GetDateTime(string columnName, IDataReader reader, DateTime defaultValue)
        {
            object objectValue = reader[columnName];
            return objectValue != null ? (DateTime)objectValue : defaultValue;
        }

        public static object GetEnum(string columnName, IDataReader reader, Type enumType, object defaultValue)
        {
            string stringValue = GetString(columnName, reader, defaultValue.ToString());
            return Enum.Parse(enumType, stringValue);
        }
    }
}

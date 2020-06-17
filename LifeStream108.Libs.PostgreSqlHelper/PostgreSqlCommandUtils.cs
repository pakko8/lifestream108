using LifeStream108.Libs.Common;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace LifeStream108.Libs.PostgreSqlHelper
{
    public static class PostgreSqlCommandUtils
    {
        public static NpgsqlParameter CreateParam(string paramName, NpgsqlDbType type, object value, int size = 0)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, type, size);
            param.Value = value;
            return param;
        }

        public static T GetEntity<T>(string query, Func<IDataReader, T> objectReader)
        {
            T resultObject = default;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        resultObject = objectReader(reader);
                    }
                }
            }
            return resultObject;
        }

        public static T[] GetEntities<T>(string query, Func<IDataReader, T> objectReader)
        {
            List<T> resultList = new List<T>();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultList.Add(objectReader(reader));
                    }
                }
            }
            return resultList.ToArray();
        }

        public static T AddEntity<T>(string query, NpgsqlParameter[] parameters)
        {
            object newIdObj;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddRange(parameters);
                connection.Open();
                newIdObj = command.ExecuteScalar();
            }
            return DataConverter.Parse<T>(newIdObj.ToString());
        }

        public static void UpdateEntity(string query, NpgsqlParameter[] parameters)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddRange(parameters);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}

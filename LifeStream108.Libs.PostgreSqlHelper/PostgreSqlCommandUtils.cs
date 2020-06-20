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
        public static NpgsqlParameter CreateParam(string paramName, object value, NpgsqlDbType type, int size = 0)
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
                connection.Open();
                resultObject = GetEntity(query, connection, objectReader);
            }
            return resultObject;
        }

        public static T GetEntity<T>(string query, NpgsqlConnection connection, Func<IDataReader, T> objectReader)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            T resultObject = default;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    resultObject = objectReader(reader);
                }
            }
            return resultObject;
        }

        public static T[] GetEntities<T>(string query, NpgsqlConnection connection, Func<IDataReader, T> objectReader,
            NpgsqlParameter[] parameters = null)
        {
            List<T> resultList = new List<T>();
            var command = connection.CreateCommand();
            command.CommandText = query;
            if (parameters != null) command.Parameters.AddRange(parameters);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    resultList.Add(objectReader(reader));
                }
            }
            return resultList.ToArray();
        }

        public static T[] GetEntities<T>(string query, Func<IDataReader, T> objectReader, NpgsqlParameter[] parameters = null)
        {
            T[] resultList;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();
                resultList = GetEntities(query, connection, objectReader, parameters);
            }
            return resultList;
        }

        public static T AddEntity<T>(string query, NpgsqlParameter[] parameters)
        {
            T newIdObj;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();
                newIdObj = AddEntity<T>(query, parameters, connection);
            }
            return newIdObj;
        }

        public static T AddEntity<T>(string query, NpgsqlParameter[] parameters, NpgsqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddRange(parameters);

            object newIdObj = command.ExecuteScalar();

            return DataConverter.Parse<T>(newIdObj.ToString());
        }

        public static void UpdateEntity(string query, NpgsqlParameter[] parameters)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();
                UpdateEntity(query, parameters, connection);
            }
        }

        public static void UpdateEntity(string query, NpgsqlParameter[] parameters, NpgsqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            if (parameters != null && parameters.Length > 0) command.Parameters.AddRange(parameters);
            command.ExecuteNonQuery();
        }
    }
}

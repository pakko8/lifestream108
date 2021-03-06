﻿using LifeStream108.Libs.Common;
using LifeStream108.Modules.SettingsManagement;
using NLog;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LifeStream108.Libs.PostgreSqlHelper
{
    public static class PostgreSqlCommandUtils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                resultObject = GetEntity(query, objectReader, connection);
            }
            return resultObject;
        }

        public static T GetEntity<T>(string query, Func<IDataReader, T> objectReader, NpgsqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            LogCommand(command);
            T resultObject = default;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    resultObject = objectReader(reader);
                }
            }
            Logger.Info($"Found item {(resultObject != null ? "not null" : "null")}");
            return resultObject;
        }

        public static T[] GetEntities<T>(string query, Func<IDataReader, T> objectReader, NpgsqlConnection connection,
            NpgsqlParameter[] parameters = null)
        {
            List<T> resultList = new List<T>();
            var command = connection.CreateCommand();
            command.CommandText = query;
            LogCommand(command);
            if (parameters != null) command.Parameters.AddRange(parameters);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    resultList.Add(objectReader(reader));
                }
            }
            Logger.Info($"Found {resultList.Count} items");
            return resultList.ToArray();
        }

        public static T[] GetEntities<T>(string query, Func<IDataReader, T> objectReader, NpgsqlParameter[] parameters = null)
        {
            T[] resultList;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();
                resultList = GetEntities(query, objectReader, connection, parameters);
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
            LogCommand(command);
            object newIdObj = command.ExecuteScalar();
            Logger.Info($"Item added with id " + newIdObj);

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
            LogCommand(command);
            command.ExecuteNonQuery();
        }

        private static void LogCommand(NpgsqlCommand command)
        {
            StringBuilder sbTrace = new StringBuilder($"Exec cmd {command.CommandText}");
            if (command.Parameters.Count > 0)
            {
                sbTrace.AppendLine(":\r\n");
                foreach (IDbDataParameter parameter in command.Parameters)
                {
                    sbTrace.AppendLine(
                        $"param '{parameter.ParameterName}' ({parameter.DbType}{(parameter.Size > 0 ? ", " + parameter.Size : "")}), Value: {parameter.Value}");
                }
            }
            Logger.Info(sbTrace.ToString());
        }
    }
}

using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LifeStream108.Modules.DictionaryManagement
{
    public static class MeasureManager
    {
        public static Measure GetMeasureByName(string measureName, int userId)
        {
            Measure measure = null;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from measures where user_id={userId} and upper(name)='{measureName}'";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        measure = ReadMeasure(reader);
                    }
                }
            }
            return measure;
        }

        public static Measure[] GetMeasuresForUser(int userId)
        {
            List<Measure> measures = new List<Measure>();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from measures where user_id={userId}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        measures.Add(ReadMeasure(reader));
                    }
                }
            }
            return measures.ToArray();
        }

        public static void AddMeasure(Measure measure)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                measure.UserCode = GetNextUserCode(measure.UserId, connection);

                var command = connection.CreateCommand();
                command.CommandText = $"insert into measures () values ()";
                command.Parameters.Add();
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static int GetNextUserCode(int userId, DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"select user_code from measures where user_id={userId} order by user_code desc limit 1";
            int nextCode = 0;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    nextCode = PgsqlUtils.GetInt("user_code", reader, 0);
                }
            }
            return nextCode + 1;
        }

        private static Measure ReadMeasure(IDataReader reader)
        {
            Measure measure = new Measure();
            measure.Id = PgsqlUtils.GetInt("id", reader, 0);
            measure.Name = PgsqlUtils.GetString("name", reader, "");
            measure.ShortName = PgsqlUtils.GetString("short_name", reader, "");
            measure.Declanation1 = PgsqlUtils.GetString("declanation1", reader, "");
            measure.Declanation2 = PgsqlUtils.GetString("declanation2", reader, "");
            measure.Declanation3 = PgsqlUtils.GetString("declanation3", reader, "");
            measure.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return measure;
        }
    }
}

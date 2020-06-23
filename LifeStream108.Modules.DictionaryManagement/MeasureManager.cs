using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;

namespace LifeStream108.Modules.DictionaryManagement
{
    public static class MeasureManager
    {
        private const string TableName = "public.measures";

        public static Measure GetMeasureByName(string measureName, int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select * from {TableName} where user_id={userId} and upper(name)='{measureName}'", ReadMeasure);
        }

        public static Measure[] GetMeasuresForUser(int userId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName} where user_id={userId}", ReadMeasure);
        }

        public static void AddMeasure(Measure measure)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                connection.Open();

                measure.UserCode = GetNextUserCode(measure.UserId, connection);

                string query =
                    $@"insert into {TableName}
                    (user_code, user_id, name, short_name, declanation1, declanation2, declanation3, reg_time)
                    values
                    (@user_code, @user_id, @name, @short_name, @declanation1, @declanation2, @declanation3, current_timestamp) returning id";

                NpgsqlParameter[] parameters = new NpgsqlParameter[]
                {
                    PostgreSqlCommandUtils.CreateParam("@user_code", measure.UserCode, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@user_id", measure.UserId, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@name", measure.Name, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@short_name", measure.ShortName, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@declanation1", measure.Declanation1, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@declanation2", measure.Declanation2, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@declanation3", measure.Declanation3, NpgsqlDbType.Varchar)
                };

                measure.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);
            }
        }

        private static int GetNextUserCode(int userId, NpgsqlConnection connection)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select user_code from {TableName} where user_id={userId} order by user_code desc limit 1",
                ReadUserCode, connection);
        }

        private static int ReadUserCode(IDataReader reader)
        {
            return PgsqlUtils.GetInt("user_code", reader, 0) + 1;
        }

        private static Measure ReadMeasure(IDataReader reader)
        {
            Measure measure = new Measure();
            measure.Id = PgsqlUtils.GetInt("id", reader);
            measure.Name = PgsqlUtils.GetString("name", reader);
            measure.ShortName = PgsqlUtils.GetString("short_name", reader);
            measure.Declanation1 = PgsqlUtils.GetString("declanation1", reader);
            measure.Declanation2 = PgsqlUtils.GetString("declanation2", reader);
            measure.Declanation3 = PgsqlUtils.GetString("declanation3", reader);
            measure.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return measure;
        }
    }
}

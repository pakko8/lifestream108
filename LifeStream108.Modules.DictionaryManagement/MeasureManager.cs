using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using Npgsql;
using System;
using System.Data;

namespace LifeStream108.Modules.DictionaryManagement
{
    public static class MeasureManager
    {
        private const string TableName = "measures";

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
            measure.UserCode = GetNextUserCode(measure.UserId);

            string query =
                $@"insert into {TableName}
                (name, short_name, declanation1, declanation2, declanation3, reg_time)
                values
                (@name, @short_name, @declanation1, @declanation2, @declanation3, current_timestamp) returning id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                    PostgreSqlCommandUtils.CreateParam("@name", DbType.String, measure.Name),
                    PostgreSqlCommandUtils.CreateParam("@declanation1", DbType.String, measure.Declanation1),
                    PostgreSqlCommandUtils.CreateParam("@declanation2", DbType.String, measure.Declanation2),
                    PostgreSqlCommandUtils.CreateParam("@declanation3", DbType.String, measure.Declanation3)
            };

            measure.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);
        }

        private static int GetNextUserCode(int userId)
        {
            return PostgreSqlCommandUtils.GetEntity(
                $"select user_code from {TableName} where user_id={userId} order by user_code desc limit 1", ReadUserCode);
        }

        private static int ReadUserCode(IDataReader reader)
        {
            return PgsqlUtils.GetInt("user_code", reader, 0) + 1;
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

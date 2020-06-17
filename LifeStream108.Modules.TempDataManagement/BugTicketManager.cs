using LifeStream108.Libs.Entities.TicketEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Data;

namespace LifeStream108.Modules.TempDataManagement
{
    public static class BugTicketManager
    {
        public static void AddBugTicket(BugTicket ticket)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    @"insert into temp_data.bug_tickets
                    (
                        error_type,
                        user_id,
                        request_details,
                        error_message,
                        reg_time,
                        fix_time,
                        notification_sent_time,
                        message_for_user,
                        status
                    )
                    values
                    (
                        @error_type,
                        @user_id,
                        @request_details,
                        @error_message,
                        current_timestamp,
                        @fix_time,
                        @notification_sent_time,
                        @message_for_user,
                        @status
                    )";
                command.Parameters.Add(new NpgsqlParameter("@error_type", DbType.String)).Value = ticket.ErrorType.ToString();
                command.Parameters.Add(new NpgsqlParameter("@user_id", DbType.Int32)).Value = ticket.UserId;
                command.Parameters.Add(new NpgsqlParameter("@request_details", DbType.String)).Value = ticket.RequestDetails;
                command.Parameters.Add(new NpgsqlParameter("@error_message", DbType.String)).Value = ticket.ErrorMessage;
                command.Parameters.Add(new NpgsqlParameter("@fix_time", DbType.DateTime)).Value = new DateTime(1800, 1, 1);
                command.Parameters.Add(new NpgsqlParameter("@notification_sent_time", DbType.DateTime)).Value = new DateTime(1800, 1, 1);
                command.Parameters.Add(new NpgsqlParameter("@message_for_user", DbType.String)).Value = ticket.MessageForUser;
                command.Parameters.Add(new NpgsqlParameter("@status", DbType.Int32)).Value = (int)ticket.Status;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}

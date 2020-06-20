using LifeStream108.Libs.Entities.TicketEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using Npgsql;
using NpgsqlTypes;
using System;

namespace LifeStream108.Modules.TempDataManagement
{
    public static class BugTicketManager
    {
        public static void AddBugTicket(BugTicket ticket)
        {
            string query =
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
                    )
                    returning id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                    PostgreSqlCommandUtils.CreateParam("@error_type", ticket.ErrorType, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@user_id", ticket.UserId, NpgsqlDbType.Integer),
                    PostgreSqlCommandUtils.CreateParam("@request_details", ticket.RequestDetails, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@error_message", ticket.ErrorMessage, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@fix_time", new DateTime(1800, 1, 1), NpgsqlDbType.Timestamp),
                    PostgreSqlCommandUtils.CreateParam("@notification_sent_time", new DateTime(1800, 1, 1), NpgsqlDbType.Timestamp),
                    PostgreSqlCommandUtils.CreateParam("@message_for_user", ticket.MessageForUser, NpgsqlDbType.Varchar),
                    PostgreSqlCommandUtils.CreateParam("@status", (int)ticket.Status, NpgsqlDbType.Integer)
            };

            ticket.Id = PostgreSqlCommandUtils.AddEntity<int>(query, parameters);
        }
    }
}

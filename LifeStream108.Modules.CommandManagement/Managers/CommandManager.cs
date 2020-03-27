using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LifeStream108.Modules.CommandManagement.Managers
{
    public static class CommandManager
    {
        public static Command[] GetCommands(int projectId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from cmd in session.Query<Command>()
                            where cmd.ProjectId == 0 || cmd.ProjectId == projectId
                            select cmd;
                return query.ToArray();
            }
        }

        public static CommandName[] GetCommandNamesForProject(int projectId)
        {
            List<CommandName> commandNames = new List<CommandName>();
            using (ISession session = HibernateLoader.CreateSession())
            {
                string commandText =
                    "select nm.* from command_names as nm " +
                    "inner join commands cmd on nm.command_id=cmd.id " +
                    $"where cmd.project_id in (0, {projectId})";
                DbCommand command = session.Connection.CreateCommand();
                command.CommandText = commandText;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        commandNames.Add(ReadCommandName(reader));
                    }
                }
            }
            return commandNames.ToArray();
        }

        public static CommandName[] GetCommandNames(int commandId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from cmdName in session.Query<CommandName>()
                            where cmdName.CommandId == commandId
                            select cmdName;
                return query.ToArray();
            }
        }

        public static (Command Command, CommandParameter[] Parameters) GetCommand(int commandId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                Command command = CommonManager<Command>.GetById(commandId, session);
                CommandParameter[] parameters = command != null ? GetParametersForCommand(command.Id, session) : null;
                return (command, parameters);
            }
        }

        private static CommandParameter[] GetParametersForCommand(int commandId, ISession session)
        {
            var query = from param in session.Query<CommandParameter>()
                        where param.CommandId == commandId
                        select param;
            return query.ToArray();
        }

        private static CommandName ReadCommandName(IDataReader reader)
        {
            CommandName commandName = new CommandName();
            commandName.Id = PgsqlUtils.GetInt("id", reader, 0);
            commandName.SortOrder = PgsqlUtils.GetInt("sort_order", reader, 0);
            commandName.CommandId = PgsqlUtils.GetInt("command_id", reader, 0);
            commandName.Alias = PgsqlUtils.GetString("alias", reader, "");
            commandName.SpacePositions = PgsqlUtils.GetString("space_positions", reader, "");
            commandName.LanguageId = PgsqlUtils.GetInt("language_id", reader, 0);
            return commandName;
        }
    }
}

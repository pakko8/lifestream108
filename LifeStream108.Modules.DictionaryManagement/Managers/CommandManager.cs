using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System;
using System.Linq;

namespace LifeStream108.Modules.DictionaryManagement.Managers
{
    public static class CommandManager
    {
        public static Command[] GetAllCommands()
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return CommonManager<Command>.GetAll(session);
            }
        }

        public static CommandName[] GetAllCommandNames()
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return CommonManager<CommandName>.GetAll(session);
            }
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

        public static Tuple<Command, CommandParameter[]> GetCommand(int commandId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                Command command = CommonManager<Command>.GetById(commandId, session);
                CommandParameter[] parameters = GetParametersForCommand(command.Id, session);
                return new Tuple<Command, CommandParameter[]>(command, parameters);
            }
        }

        private static CommandParameter[] GetParametersForCommand(int commandId, ISession session)
        {
            var query = from param in session.Query<CommandParameter>()
                        where param.CommandId == commandId
                        select param;
            return query.ToArray();
        }
    }
}

using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LifeStream108.Libs.Entities;
using LifeStream108.Modules.CommandManagement;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class CommandListProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            Command[] commands = CommandManager.GetCommands(session.ProjectId).Where(n => n.Active).ToArray();
            commands = PickupAppropriateCommands(commands, session);
            CommandName[] allCommandNames = CommandManager.GetCommandNamesForProject(session.ProjectId);

            StringBuilder sbCommandsInfo = new StringBuilder();
            for (int cmdIndex = 0; cmdIndex < commands.Length; cmdIndex++)
            {
                Command currentCommand = commands[cmdIndex];
                sbCommandsInfo.Append($"<b>{currentCommand.Name}</b>");
                if (!string.IsNullOrEmpty(currentCommand.Description)) sbCommandsInfo.Append($"\r\n<i>{currentCommand.Description}</i>");
                CommandName[] thisCommandNames = allCommandNames.Where(n => n.CommandId == currentCommand.Id).OrderBy(n => n.SortOrder).ToArray();
                if (thisCommandNames.Length > 0)
                {
                    sbCommandsInfo.Append("\r\nКоманды: ");
                    for (int cmdNameIndex = 0; cmdNameIndex < thisCommandNames.Length; cmdNameIndex++)
                    {
                        sbCommandsInfo.Append($"<b>{thisCommandNames[cmdNameIndex].GetReadableAias()}</b>");
                        if (cmdNameIndex < thisCommandNames.Length - 1) sbCommandsInfo.Append(", ");
                        else sbCommandsInfo.Append(".");
                    }
                }

                if (cmdIndex < commands.Length - 1) sbCommandsInfo.Append("\r\n\r\n");
            }

            return ExecuteCommandResult.CreateSuccessObject(sbCommandsInfo.ToString());
        }

        private static Command[] PickupAppropriateCommands(Command[] commands, Session userSession)
        {
            // TODO Think over algorithm
            List<Command> resultCommands = new List<Command>();
            if (userSession.LastLifeActivityId > 0)
                resultCommands.AddRange(commands.Where(n => n.EntityType == EntityType.LifeActivity));
            else if (userSession.LastLifeGroupId > 0)
                resultCommands.AddRange(commands.Where(n => n.EntityType == EntityType.LifeGroup));
            else
                resultCommands.AddRange(commands.Where(n => n.EntityType == EntityType.NoEntity || n. EntityType == EntityType.All));

            resultCommands = resultCommands.OrderBy(n => n.SortOrder).ToList();

            resultCommands.AddRange(commands.Where(n => n.EntityType == EntityType.ToDo));
            return resultCommands.ToArray();
        }
    }
}

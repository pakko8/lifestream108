using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Modules.DictionaryManagement.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LifeStream108.Libs.Entities;
using LifeStream108.Modules.CommandManagement.Managers;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class CommandListProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            Command[] commands = CommandManager.GetAllCommands();
            commands = PickupAppropriateCommands(commands, session);
            CommandName[] allCommandNames = CommandManager.GetAllCommandNames();

            StringBuilder sbCommandsInfo = new StringBuilder();
            Command[] availableCommands = commands.Where(n => n.Active).OrderBy(n => n.SortOrder).ToArray();
            for (int cmdIndex = 0; cmdIndex < availableCommands.Length; cmdIndex++)
            {
                Command currentCommand = availableCommands[cmdIndex];
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

                if (cmdIndex < availableCommands.Length - 1) sbCommandsInfo.Append("\r\n\r\n");
            }

            return ExecuteCommandResult.CreateSuccessObject(sbCommandsInfo.ToString());
        }

        private static Command[] PickupAppropriateCommands(Command[] commands, Session userSession)
        {
            List<Command> resultCommands = new List<Command>();
            if (userSession.LastLifeActivityId > 0) resultCommands.AddRange(commands.Where(n => n.EntityType == EntityType.LifeActivity));
            else if (userSession.LastLifeGroupId > 0) resultCommands.AddRange(commands.Where(n => n.EntityType == EntityType.LifeGroup));
            else resultCommands.AddRange(commands.Where(n => n.EntityType == EntityType.NoEntity || n. EntityType == EntityType.All));
            return resultCommands.ToArray();
        }
    }
}

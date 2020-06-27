using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Modules.CommandManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class CommandTester
    {
        private const int Project = 1;

        public static void Run()
        {
            Command[] commands = CommandManager.GetCommands(Project);
            Assert.IsTrue(commands.Length > 0, "Command list is empty");

            Command command = commands[0];
            CommandName[] commandNames = CommandManager.GetCommandNames(command.Id);
            Assert.IsTrue(commandNames.Length > 0, $"No names for command {command.Id}");

            CommandName[] projCommandNames = CommandManager.GetCommandNamesForProject(Project);
            Assert.IsTrue(projCommandNames.Length > 0, $"No command names for project {Project}");
        }
    }
}

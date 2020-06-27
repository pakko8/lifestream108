using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Modules.CommandProcessors;
using NUnit.Framework;
using System;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class ProcessCommandTester
    {
        public static void Run()
        {
            Session session = new Session
            {
                UserId = UserTester.UserId_1,
                ProjectId = 1
            };

            string request = $"log:1;{DateTime.Now:dd.MM.yyyy};60+2+10";
            CommandExecutor commandExecututor = new CommandExecutor();
            ExecuteCommandResult execResult = commandExecututor.Run(request, session);
            Console.WriteLine(execResult.Success ? execResult.ResponseMessage : execResult.ErrorText);

            Assert.IsTrue(execResult.Success);
        }
    }
}

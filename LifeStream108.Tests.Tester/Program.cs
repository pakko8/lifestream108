using LifeStream108.Libs.Entities;
using LifeStream108.Modules.NewsProcessors;
using LifeStream108.Modules.TelegramBotManager;
using System;

namespace LifeStream108.Tests.Tester
{
    internal static class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                /*ActivitiesImportTuner.Run();
                return;*/

                /*ActionLogsImporter.Run();
                return;*/

                /*ViomsProcessor viomsProcessor = new ViomsProcessor();
                NewsItem[] newsList = viomsProcessor.GetLastNews(@"https://www.vioms.ru/email_lists/79", 1, DateTime.Now.AddMonths(-1));*/

                /*CommandProcessor commandProcessor = new CommandProcessor();
                var commandExecuteResult = commandProcessor.Process("2");*/

                MainTelegramChatClient chatClient = new MainTelegramChatClient();
                chatClient.Start();
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();

                chatClient.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
                return;
            }
        }
    }
}

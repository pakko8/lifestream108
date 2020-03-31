using LifeStream108.Libs.Common;
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
                DateTime initialTime = new DateTime(2020, 4, 1);
                DateTime lastRemindTime = new DateTime(2020, 4, 10);
                DateTime zeroTime = lastRemindTime;
                int repeatValue = 9;
                DateTime now = new DateTime(2020, 9, 30);

                int countTimes = (int)Math.Floor((now - zeroTime).TotalDays / repeatValue);
                DateTime estimatedTime = zeroTime.AddDays(countTimes * repeatValue);
                if (estimatedTime < now) estimatedTime = estimatedTime.AddDays(repeatValue);
                int a = 10;
                /*
                // ClientSecret = "2e780b443420a51b8e76d711eceaf1184a1294ce67b17827014705742eda";
                WunderlistImporter importer = new WunderlistImporter(
                    "94dd7218629ece859bdf", "9229e8a756c4c068ce3bdc44fe68fdaef2849eb11f2d5f51cc6feddeb727",
                    "Personal Tasks", "alexx.silver@gmail.com");
                importer.Run();

                // ClientSecret = aa8f82b0411ea0d14c5611ae98a35c0b968748f3cd3df1d934a7fc48be01
                WunderlistImporter importer2 = new WunderlistImporter(
                    "df752954852e2cbb4a6d", "8897fdd0f2f71a394a44ac78fd0eb6b657d2171fe9df6d351646cd833b5a",
                    "Work Tasks", "aserebryakov@rps.kz");
                importer2.Run();
                Console.WriteLine("Finished");
                Console.ReadKey();
                return;
                */

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

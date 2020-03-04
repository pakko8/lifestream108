using System.ServiceProcess;

namespace LifeStream108.Services.TelegramBotService
{
    internal static class Program
    {
        public static void Main()
        {
            ServiceBase.Run(new MainService());
        }
    }
}

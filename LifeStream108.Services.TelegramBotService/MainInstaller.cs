using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace LifeStream108.Services.TelegramBotService
{
    [RunInstaller(true)]
    public class MainInstaller : Installer
    {
        public MainInstaller()
        {
            Installers.AddRange(new Installer[]
            {
                new ServiceProcessInstaller
                {
                    Account = ServiceAccount.LocalSystem,
                    Password = null,
                    Username = null
                },
                new ServiceInstaller
                {
                    Description = "LifeStream108 Telegram Bot",
                    DisplayName = "LifeStream108.TelegramBot",
                    ServiceName = "LifeStream108.TelegramBot",
                    StartType = ServiceStartMode.Automatic
                }
            }); ;
        }
    }
}

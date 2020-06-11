using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LifeStream108.Services.NewsCheckerWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                InnerHelpers.Logger.Error("Error to launch service: " + ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    AppSettings appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
                    services.AddSingleton(appSettings);

                    services.AddHostedService<Worker>();
                });
    }
}

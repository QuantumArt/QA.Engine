using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace QA.DotNetCore.OnScreenAdmin.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddConsole();
                })
                .UseNLog()  // NLog: setup NLog for Dependency injection
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .Build();
    }
}

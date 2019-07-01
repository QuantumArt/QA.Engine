using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace DemoWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuringFileName = "nlog.config";

            var aspnetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var environmentSpecificLogFileName = $"nlog.{aspnetEnvironment}.config";

            if (File.Exists(environmentSpecificLogFileName))
            {
                configuringFileName = environmentSpecificLogFileName;
            }
            var logger = NLogBuilder.ConfigureNLog(configuringFileName).GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()  // NLog: setup NLog for Dependency injection
                .Build();
    }
}

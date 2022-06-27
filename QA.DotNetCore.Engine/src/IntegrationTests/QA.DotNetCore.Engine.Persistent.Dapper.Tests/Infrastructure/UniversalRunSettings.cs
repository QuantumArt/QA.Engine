using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure
{
    public class UniversalRunSettings : IRunSettings
    {
        private readonly IConfiguration _configuration;

        public UniversalRunSettings(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public static UniversalRunSettings Load()
        {
            string runSettingsPath = Environment.GetEnvironmentVariable("RunSettingsFile");

            string workingDirectory = Directory.GetCurrentDirectory();

            runSettingsPath ??= new DirectoryInfo(workingDirectory)
                .GetFiles("*.runsettings")
                .First()
                .Name;

            Console.WriteLine($"Use settings from '{runSettingsPath}'.");

            var configurationBuilder = new ConfigurationBuilder()
                .AddXmlFile(runSettingsPath, optional: false);

            return new UniversalRunSettings(configurationBuilder.Build());
        }

        public string Get(string name, string defaultValue = null)
        {
            return _configuration.GetSection($"TestRunParameters:Parameter:{name}:value").Value ?? defaultValue;
        }
    }
}

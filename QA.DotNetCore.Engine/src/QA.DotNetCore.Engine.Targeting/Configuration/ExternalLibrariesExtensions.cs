using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Targeting.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QA.DotNetCore.Engine.Targeting.Configuration
{
    public static class ExternalLibrariesExtensions
    {
        public static void AddExternalTargeting(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("TargetingSettings");
            services.Configure<TargetingFilterSettings>(section);
            var settings = section.Get<TargetingFilterSettings>();

            foreach(var registration in settings.GetTargetingRegistrations())
            {
                registration.RegisterTargetingServices(services, configuration);
            }
        }

        public static IApplicationBuilder UseExternalTargeting(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<TargetingFilterSettings>>();

            foreach (var registration in options.Value.GetTargetingRegistrations())
            {
                registration.ConfigureTargeting(app);
            }

            return app;
        }

        private static ITargetingRegistration[] GetTargetingRegistrations(this TargetingFilterSettings settings) => settings
            .TargetingLibraries
            .LoadExternalLibraries()
            .Select(GetTargetingRegistration)
            .Where(item => item != null)
            .ToArray();

        private static ITargetingRegistration GetTargetingRegistration(this Assembly assembly)
        {
            foreach (var implementationType in assembly.GetExportedTypes())
            {
                if (typeof(ITargetingRegistration).IsAssignableFrom(implementationType) && implementationType.IsClass && !implementationType.IsAbstract)
                {
                    return (ITargetingRegistration)Activator.CreateInstance(implementationType);
                }
            }

            return null;
        }

        private static Assembly[] LoadExternalLibraries(this string[] libraries)
        {
            var assemblies = new List<Assembly>();

            if (libraries != null)
            {
                foreach (var library in libraries)
                {
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{library}.dll");

                    try
                    {
                        var assembly = Assembly.LoadFile(path);
                        assemblies.Add(assembly);
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return assemblies.ToArray();
        }
    }
}

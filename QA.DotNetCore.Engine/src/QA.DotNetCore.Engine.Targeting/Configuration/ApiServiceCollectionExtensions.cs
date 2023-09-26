using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Targeting.Factories;
using QA.DotNetCore.Engine.Targeting.Settings;
using System;
using System.IO;
using System.Reflection;

namespace QA.DotNetCore.Engine.Targeting.Configuration
{
    public static class ApiServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для таргетирования в IServiceCollection
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <param name="configuration">Конфигурация</param>
        /// <returns></returns>
        public static IServiceCollection AddApiTargeting(this IServiceCollection services, IConfiguration configuration)
        {
            var _ = services.Configure<TargetingFilterSettings>(configuration.GetSection("TargetingFilterSettings"));
            var settings = configuration.GetSection("TargetingFilterSettings").Get<TargetingFilterSettings>();
            var assembly = settings.LoadTargetingLibrary();
            var factory = settings.GetFactory();

            if (factory != null)
            {
                services.AddTargetingFactory(factory);
            }
            else if (!services.DiscoverTargetingFactory(assembly))
            {
                services.TryAddSingleton<ITargetingFiltersFactory, EmptyTargetingFiltersFactory>();
            }
                        
            return services;
        }

        private static void AddTargetingFactory(this IServiceCollection services, Type factory) =>
            services.TryAddSingleton(typeof(ITargetingFiltersFactory), factory);

        private static bool DiscoverTargetingFactory(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (typeof(ITargetingFiltersFactory).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                {
                    services.AddTargetingFactory(type);
                    return true;
                }
            }
            return false;
        }

        private static Type GetFactory(this TargetingFilterSettings settings) =>
            Type.GetType(settings?.TargetingFiltersFactory);

        private static Assembly LoadTargetingLibrary(this TargetingFilterSettings settings)
        {
            if (settings?.TargetingLibrary != null)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{settings.TargetingLibrary}.dll");

                try
                {
                    var assembly = Assembly.LoadFile(path);
                    return assembly;
                }
                catch (ArgumentException)
                {
                }
            }

            return null;
        }
    }
}

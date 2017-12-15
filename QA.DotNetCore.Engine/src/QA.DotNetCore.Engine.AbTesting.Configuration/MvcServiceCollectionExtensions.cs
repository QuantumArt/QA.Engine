using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using System;

namespace QA.DotNetCore.Engine.AbTesting.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для AB-тестов в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        public static AbTestServicesConfigurator AddAbTestServices(this IServiceCollection services)
        {
            return AddAbTestServices(services, null);
        }

        /// <summary>
        /// Добавление сервисов для AB-тестов в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="setupAction">действие для изменения предзаданных параметров аб-тестов</param>
        public static AbTestServicesConfigurator AddAbTestServices(this IServiceCollection services, Action<AbTestOptions> setupAction)
        {
            return new AbTestServicesConfigurator(services, setupAction);
        }
    }
}

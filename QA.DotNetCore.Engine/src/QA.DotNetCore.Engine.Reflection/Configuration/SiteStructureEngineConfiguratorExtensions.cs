using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Reflection.Configuration
{
    public static class SiteStructureEngineConfiguratorExtensions
    {
        /// <summary>
        /// Зарегистрировать тривиальную реализацию ITypeFinder, которая ищет типы только в одной сборке - той же, что и объект sample
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static ISiteStructureEngineConfigurator AddSingleAssemblyTypeFinder(this ISiteStructureEngineConfigurator cfg, object sample)
        {
            cfg.Services.Add(new ServiceDescriptor(typeof(ITypeFinder), provider => new SingleAssemblyTypeFinder(sample), ServiceLifetime.Singleton));
            return cfg;
        }
    }
}

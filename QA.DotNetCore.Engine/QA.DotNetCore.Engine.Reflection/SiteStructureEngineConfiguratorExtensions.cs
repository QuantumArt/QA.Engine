using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Reflection
{
    public static class SiteStructureEngineConfiguratorExtensions
    {
        public static ISiteStructureEngineConfigurator AddSingleAssemblyTypeFinder(this ISiteStructureEngineConfigurator cfg, object sample)
        {
            cfg.Services.Add(new ServiceDescriptor(typeof(ITypeFinder), provider => new SingleAssemblyTypeFinder(sample), ServiceLifetime.Singleton));
            return cfg;
        }
    }
}

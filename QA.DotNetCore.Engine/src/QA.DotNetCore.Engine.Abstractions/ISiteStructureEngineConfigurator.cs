using Microsoft.Extensions.DependencyInjection;

namespace QA.DotNetCore.Engine.Abstractions
{
    public interface ISiteStructureEngineConfigurator
    {
        IServiceCollection Services { get; }
    }
}

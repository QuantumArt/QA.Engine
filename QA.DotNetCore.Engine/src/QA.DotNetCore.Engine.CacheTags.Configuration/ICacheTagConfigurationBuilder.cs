using Microsoft.Extensions.DependencyInjection;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    public interface ICacheTagConfigurationBuilder
    {
        IServiceCollection Services { get; }
    }
}

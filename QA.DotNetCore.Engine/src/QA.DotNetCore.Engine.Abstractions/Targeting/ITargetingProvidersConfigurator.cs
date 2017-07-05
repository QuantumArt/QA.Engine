using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingProvidersConfigurator
    {
        IServiceProvider ServiceProvider { get; }
        void Add<T>() where T : ITargetingProvider;
        void Add(ITargetingProvider provider);
        IEnumerable<ITargetingProvider> GetProviders();
    }
}

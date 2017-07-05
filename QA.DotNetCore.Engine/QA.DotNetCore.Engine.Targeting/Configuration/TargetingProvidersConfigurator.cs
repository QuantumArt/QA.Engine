using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Targeting.Configuration
{
    public class TargetingProvidersConfigurator : ITargetingProvidersConfigurator
    {
        readonly IServiceProvider _serviceProvider;
        readonly IList<ITargetingProvider> _providers = new List<ITargetingProvider>();

        public TargetingProvidersConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public void Add<T>() where T : ITargetingProvider
        {
            var provider = (T)_serviceProvider.GetRequiredService(typeof(T));
            if (provider == null)
                throw new Exception($"TargetingConfigurationBuilder: Type {typeof(T).Name} not found in IoC! ");
            _providers.Add(provider);
        }

        public void Add(ITargetingProvider provider)
        {
            _providers.Add(provider);
        }

        public IEnumerable<ITargetingProvider> GetProviders()
        {
            return _providers;
        }
    }
}
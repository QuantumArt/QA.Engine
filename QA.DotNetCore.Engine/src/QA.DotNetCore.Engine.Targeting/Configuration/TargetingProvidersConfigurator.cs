using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Targeting.Configuration
{
    public class TargetingProvidersConfigurator : ITargetingProvidersConfigurator
    {
        readonly IServiceProvider _serviceProvider;
        readonly IList<ITargetingProvider> _providers = new List<ITargetingProvider>();
        readonly IList<Type> _providerTypes = new List<Type>();
        readonly IList<ITargetingPossibleValuesProvider> _possibleValuesProviders = new List<ITargetingPossibleValuesProvider>();
        readonly IList<Type> _possibleValuesProviderTypes = new List<Type>();

        public TargetingProvidersConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public void Add<T>() where T : ITargetingProvider
        {
            _providerTypes.Add(typeof(T));
        }

        public void Add(ITargetingProvider provider)
        {
            _providers.Add(provider);
        }

        public void AddPossibleValues<T>() where T : ITargetingPossibleValuesProvider
        {
            _possibleValuesProviderTypes.Add(typeof(T));
        }

        public void AddPossibleValues(ITargetingPossibleValuesProvider provider)
        {
            _possibleValuesProviders.Add(provider);
        }

        public IEnumerable<ITargetingPossibleValuesProvider> GetPossibleValuesProviders()
        {
            return _possibleValuesProviders.Concat(_possibleValuesProviderTypes.Select(t => (ITargetingPossibleValuesProvider)_serviceProvider.GetRequiredService(t)));
        }

        public IEnumerable<ITargetingProvider> GetProviders()
        {
            return _providers.Concat(_providerTypes.Select(t => (ITargetingProvider)_serviceProvider.GetRequiredService(t)));
        }
    }
}

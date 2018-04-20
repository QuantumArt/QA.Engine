using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Targeting.Configuration
{
    public class TargetingFiltersConfigurator : ITargetingFiltersConfigurator
    {
        readonly IServiceProvider _serviceProvider;
        readonly IList<ITargetingFilter> _filters = new List<ITargetingFilter>();
        readonly IList<Type> _filterTypes = new List<Type>();

        public TargetingFiltersConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public ITargetingFilter ResultFilter
        {
            get
            {
                return new UnitedFilter(_filters.Concat(_filterTypes.Select(t => (ITargetingFilter)_serviceProvider.GetRequiredService(t))));
            }
        }

        public void Add<T>() where T : ITargetingFilter
        {
            _filterTypes.Add(typeof(T));
        }

        public void Add(ITargetingFilter filter)
        {
            _filters.Add(filter);
        }
    }
}


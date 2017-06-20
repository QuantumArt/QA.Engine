using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Targeting.Filters
{
    public class TargetingFiltersConfigurator : ITargetingFiltersConfigurator
    {
        readonly IServiceProvider _serviceProvider;
        readonly IList<ITargetingFilter> _filters = new List<ITargetingFilter>();

        public TargetingFiltersConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public ITargetingFilter ResultFilter { get; private set; }

        public void Add<T>() where T : ITargetingFilter
        {
            var filter = (T)_serviceProvider.GetRequiredService(typeof(T));
            if (filter == null)
                throw new Exception($"FiltersConfigurationBuilder: Type {typeof(T).Name} not found in IoC! ");
            _filters.Add(filter);
            ResultFilter = new UnitedFilter(_filters);
        }

        public void Add(ITargetingFilter filter)
        {
            _filters.Add(filter);
            ResultFilter = new UnitedFilter(_filters);
        }
    }

    public static class MvcApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSiteSctructureFilters(this IApplicationBuilder app, Action<ITargetingFiltersConfigurator> configureFilters)
        {
            var builder = app.ApplicationServices.GetRequiredService<ITargetingFiltersConfigurator>();
            configureFilters(builder);
            return app;
        }
    }
}


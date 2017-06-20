using System;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingFiltersConfigurator
    {
        IServiceProvider ServiceProvider { get; }
        void Add<T>() where T : ITargetingFilter;
        void Add(ITargetingFilter filter);
        ITargetingFilter ResultFilter { get; }
    }
}

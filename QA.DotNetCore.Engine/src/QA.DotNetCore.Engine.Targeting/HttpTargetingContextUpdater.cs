using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Targeting
{
    public class HttpTargetingContextUpdater : ITargetingContextUpdater
    {
        private readonly record struct TargetingConfiguration(IDictionary<string, object> Values, TargetingSource Source);

        private readonly ServiceSetConfigurator<ITargetingProvider> _targetingConfigurationBuilder;
        private readonly ServiceSetConfigurator<ITargetingProviderAsync> _targetingConfigurationBuilderForAsync;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpTargetingContextUpdater(
            ServiceSetConfigurator<ITargetingProvider> p,
            ServiceSetConfigurator<ITargetingProviderAsync> p2,
            IHttpContextAccessor httpContextAccessor)
        {
            _targetingConfigurationBuilder = p;
            _targetingConfigurationBuilderForAsync = p2;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Update(IDictionary<string, string> values)
        {
            var context = _httpContextAccessor.HttpContext;

            var startValues = values == null ?
                new TargetingConfiguration[0] :
                new TargetingConfiguration[] { new TargetingConfiguration(values.ToDictionary(pair => pair.Key, pair => (object)pair.Value), TargetingSource.Primary) };

            //сохраним в HttpContext все значения таргетирования, как из синхронных провайдеров, так и асинхронных
            var targetingKeys = new List<string>();

            var getValuesTasks = _targetingConfigurationBuilderForAsync.GetServices(context.RequestServices)
                .Select(async tp => new TargetingConfiguration(await tp.GetValues(), tp.Source));
            await Task.WhenAll(getValuesTasks);

            foreach (var targeting in startValues
                .Union(getValuesTasks.Select(t => t.Result))                
                .Union(_targetingConfigurationBuilder.GetServices(context.RequestServices).Select(tp => new TargetingConfiguration(tp.GetValues(), tp.Source)))
                .Where(targeting => targeting.Values != null))
            {
                foreach (var key in targeting.Values.Keys)
                {
                    if (targeting.Source == TargetingSource.Primary)
                    {
                        context.Items[HttpTargetingContext.TargetingPrimaryKey + key] = targeting.Values[key];
                    }

                    context.Items[key] = targeting.Values[key];
                    targetingKeys.Add(key);
                }
            }

            //сохраним в HttpContext все полученные ключи таргетирования
            context.Items[HttpTargetingContext.TargetingKeysContextKey] = targetingKeys.Distinct().ToArray();
        }
    }
}

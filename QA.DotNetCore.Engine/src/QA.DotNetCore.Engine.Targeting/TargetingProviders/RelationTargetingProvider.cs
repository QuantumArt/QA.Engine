using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Targeting.TargetingProviders
{
    /// <summary>
    /// Должен вызываться после <typeparamref name="UrlTokenTargetingProvider"/>
    /// Преобразует алиас таргетинг поля в список id (связанных родилельской иерархией, вплоть до корневого, если справочник поддерживает иерархию)
    /// </summary>
    public abstract class RelationTargetingProvider : ITargetingProvider
    {
        protected abstract string TargetingKey { get; }        
        private readonly ITargetingContext _context;
        private readonly IDictionaryProvider _dictionaryProvider;
        public TargetingSource Source => TargetingSource.Chained;

        public RelationTargetingProvider(ITargetingContext context, IDictionaryProvider dictionaryProvider)
        {
            _context = context;
            _dictionaryProvider = dictionaryProvider;
        }

        public IDictionary<string, object> GetValues()
        {
            if (_dictionaryProvider.GetKeys().Contains(TargetingKey))
            {
                var targetingValue = _context.GetTargetingValue(TargetingKey);

                if (targetingValue is string targetingALias && targetingALias != null)
                {
                    var items = _dictionaryProvider.GetParentDictionaryItems(TargetingKey, targetingALias);
                    var ids = items.Select(x => x.Id);
                    var idsValue = string.Join(",", ids);
                    return new Dictionary<string, object> { { TargetingKey, idsValue } };
                }
            }

            return new Dictionary<string, object>();
        }
    }
}

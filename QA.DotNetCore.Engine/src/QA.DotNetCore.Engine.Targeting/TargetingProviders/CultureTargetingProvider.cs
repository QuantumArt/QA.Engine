using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData.Interfaces;

namespace QA.DotNetCore.Engine.Targeting.TargetingProviders
{
    public class CultureTargetingProvider : RelationTargetingProvider
    {
        protected override string TargetingKey => "culture";

        public CultureTargetingProvider(ITargetingContext context, IDictionaryProvider dictionaryProvider)
            : base(context, dictionaryProvider)
        {
        }
    }
}

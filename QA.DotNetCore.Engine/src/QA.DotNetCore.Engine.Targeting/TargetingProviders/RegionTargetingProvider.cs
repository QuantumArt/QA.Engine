using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData.Interfaces;

namespace QA.DotNetCore.Engine.Targeting.TargetingProviders
{
    public class RegionTargetingProvider : RelationTargetingProvider
    {
        protected override string TargetingKey => "region";

        public RegionTargetingProvider(ITargetingContext context, IDictionaryProvider dictionaryProvider)
            : base(context, dictionaryProvider)
        {
        }        
    }
}

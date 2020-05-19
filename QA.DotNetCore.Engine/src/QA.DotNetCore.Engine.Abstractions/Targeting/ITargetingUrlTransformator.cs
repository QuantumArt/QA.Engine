using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingUrlTransformator
    {
        string AddCurrentTargetingValuesToUrl(string originalUrl);
    }
}

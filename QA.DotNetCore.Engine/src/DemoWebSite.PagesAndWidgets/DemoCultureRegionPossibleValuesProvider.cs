using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;

namespace DemoWebSite.PagesAndWidgets
{
    public class DemoCultureRegionPossibleValuesProvider : ITargetingPossibleValuesProvider
    {
        public IDictionary<string, IEnumerable<object>> GetPossibleValues()
        {
            return new Dictionary<string, IEnumerable<object>>
            {
                ["culture"] = new[] { "ru-ru", "en-us" },
                ["region"] = new[] { "moskva", "spb" }
            };
        }
    }
}

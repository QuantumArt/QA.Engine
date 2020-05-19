using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using System.Collections.Generic;

namespace DemoWebSite.PagesAndWidgets
{
    public class DemoCultureRegionPossibleValuesProvider : IHeadTokenPossibleValuesProvider
    {
        public IDictionary<string, IEnumerable<string>> GetPossibleValues()
        {
            return new Dictionary<string, IEnumerable<string>>
            {
                ["culture"] = new[] { "ru-ru", "en-us" },
                ["region"] = new[] { "moskva", "spb" }
            };
        }
    }
}

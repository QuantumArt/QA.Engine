using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching
{
    public interface IHeadTokenPossibleValuesProvider
    {
        IDictionary<string, IEnumerable<string>> GetPossibleValues();
    }
}

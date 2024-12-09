using System.Linq;
using Microsoft.AspNetCore.Http;

namespace QA.DotNetCore.Engine.Routing.Configuration;

public class ExcludePathChecker
{
    private readonly string[] _excludePaths;

    public ExcludePathChecker(string[] excludePaths = null)
    {
        _excludePaths = excludePaths;
    }

    public bool IsExcluded(PathString pathToCheck)
    {
        if (_excludePaths == null || _excludePaths.Length == 0)
        {
            return false;
        }

        return _excludePaths.Any(excludePath => pathToCheck.StartsWithSegments(excludePath));
    }
}

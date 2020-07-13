using System;

namespace QA.DotNetCore.Engine.Abstractions.Wildcard
{
    [Flags]
    public enum WildcardMatchingOption
    {
        None = 0,
        StartsWith = 1,
        EndsWith = 2,
        FullMatch = WildcardMatchingOption.StartsWith | WildcardMatchingOption.EndsWith,
        CaseSensitive = 4
    }
}

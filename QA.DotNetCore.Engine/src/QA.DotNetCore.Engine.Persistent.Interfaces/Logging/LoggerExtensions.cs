using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Logging;

public static class LoggerExtensions
{
    public static IDisposable BeginScopeWith(this ILogger logger, params (string key, object value)[] keys)
    {
        return logger.BeginScope(keys.ToDictionary(x => x.key, x => x.value));
    }
}

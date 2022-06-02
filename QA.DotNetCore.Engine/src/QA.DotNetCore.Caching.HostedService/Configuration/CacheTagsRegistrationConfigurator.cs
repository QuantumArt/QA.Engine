using System;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    public class CacheTagsRegistrationConfigurator
    {
        public bool UseTimer { get; private set; }
        public TimeSpan TimerInterval { get; private set; } = TimeSpan.FromSeconds(30);
        public bool UseMiddleware { get; private set; }
        public string ExcludeRequestPathRegex { get; private set; }

        public void InvalidateByTimer(TimeSpan interval = default)
        {
            UseTimer = true;
            if (interval != default)
            {
                TimerInterval = interval;
            }
        }

        public void InvalidateByMiddleware(string excludeRequestPathRegex)
        {
            UseMiddleware = true;
            ExcludeRequestPathRegex = excludeRequestPathRegex;
        }
    }
}

using System;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    public static class AbstractItemExtensions
    {
        private static readonly char[] _arrayDelimiters = new char[] { '\n', '\r', ';', ' ', ',' };

        public static string[] GetBindings(this AbstractItem item)
        {
            var dnsBindings = item.GetDetail<string>("Bindings", null);
            if (dnsBindings != null)
            {
                return dnsBindings
                    .Split(_arrayDelimiters, StringSplitOptions.RemoveEmptyEntries)
                    .Select(_ => _.Trim())
                    .ToArray();
            }
            return Array.Empty<string>();
        }

        public static string[] GetAllowedUrlPatterns(this AbstractItem item)
        {
            var patterns = item.GetDetail<string>("AllowedUrlPatterns", null);
            if (patterns != null)
            {
                return patterns
                    .Split(_arrayDelimiters, StringSplitOptions.RemoveEmptyEntries)
                    .Select(_ => _.Trim())
                    .ToArray();
            }
            return Array.Empty<string>();
        }

        public static string[] GetDeniedUrlPatterns(this AbstractItem item)
        {
            var patterns = item.GetDetail<string>("DeniedUrlPatterns", null);
            if (patterns != null)
            {
                return patterns
                    .Split(_arrayDelimiters, StringSplitOptions.RemoveEmptyEntries)
                    .Select(_ => _.Trim())
                    .ToArray();
            }
            return Array.Empty<string>();
        }
    }
}

using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Linq;
using QA.DotNetCore.Engine.Abstractions.Wildcard;

namespace QA.DotNetCore.Engine.Widgets
{
    /// <summary>
    /// Фильтр, использующийся для отображения виджетов в зоне
    /// </summary>
    public class WidgetFilter : BaseTargetingFilter
    {
        readonly string _zone;
        readonly string _url;

        public WidgetFilter(string zone, string url)
        {
            _zone = zone;
            _url = url.TrimEnd('/');
        }

        public override bool Match(IAbstractItem item)
        {
            if (item.IsPage)
                return false;

            IAbstractWidget widget = item as IAbstractWidget;
            if (widget == null)
                return false;

            if (!string.Equals(widget.ZoneName, _zone))
                return false;

            if ((widget.AllowedUrlPatterns == null || widget.AllowedUrlPatterns.Length == 0)
                && (widget.DeniedUrlPatterns == null || widget.DeniedUrlPatterns.Length == 0))
                return true;

            if (widget.DeniedUrlPatterns != null && widget.DeniedUrlPatterns.Any())
            {
                var deniedMatcher = new WildcardMatcher(WildcardMatchingOption.FullMatch, widget.DeniedUrlPatterns);
                if (deniedMatcher.Match(_url).Any())
                    return false;
            }

            if (widget.AllowedUrlPatterns != null && widget.AllowedUrlPatterns.Any())
            {
                var allowedMatcher = new WildcardMatcher(WildcardMatchingOption.FullMatch, widget.AllowedUrlPatterns);
                return allowedMatcher.Match(_url).Any();
            }

            return true;
        }
    }
}

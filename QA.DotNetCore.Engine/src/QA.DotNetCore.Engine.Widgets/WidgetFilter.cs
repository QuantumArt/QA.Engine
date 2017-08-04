using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Linq;

namespace QA.DotNetCore.Engine.Widgets
{
    /// <summary>
    /// Фильтр, использующийся для отображения виджетов в зоне
    /// </summary>
    public class WidgetFilter : BaseTargetingFilter
    {
        string _zone;
        string _url;

        public WidgetFilter(string zone, string url)
        {
            _zone = zone;
            _url = url;
        }

        public override bool Match(IAbstractItem item)
        {
            if (item.IsPage)
                return true;

            IAbstractWidget widget = item as IAbstractWidget;
            if (widget == null)
                return true;

            if (!String.Equals(widget.ZoneName, _zone))
                return false;

            if (widget.AllowedUrlPatterns == null && widget.DeniedUrlPatterns == null)
                return true;

            if (widget.DeniedUrlPatterns != null)
            {
                foreach (var pattern in widget.DeniedUrlPatterns)
                {
                    if (IsMatchPattern(_url, pattern))
                        return false;
                }
            }

            if (widget.AllowedUrlPatterns != null)
            {
                foreach (var pattern in widget.AllowedUrlPatterns)
                {
                    if (IsMatchPattern(_url, pattern))
                        return true;
                }

                if (widget.AllowedUrlPatterns.Any())
                    return false;
            }

            return true;
        }

        protected virtual bool IsMatchPattern(string url, string pattern)
        {
            if (pattern.EndsWith("*"))
            {
                var p = pattern.TrimEnd('*').TrimEnd('/').TrimStart('/');
                return url.TrimEnd('/').TrimStart('/').ToLower().StartsWith(p.ToLower());
            }
            else
            {
                var u = url.TrimEnd('/').TrimStart('/');
                var p = pattern.TrimEnd('/').TrimStart('/');

                return u.Equals(p, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}

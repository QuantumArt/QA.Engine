using System;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.QpData.Replacements
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class LibraryUrlAttribute : Attribute, ILoaderOption
    {
        public Type Type { get; private set; }

        public string PropertyName { get; private set; }

        /// <summary>
        /// Id контента-расширения (extension)
        /// </summary>
        public int ContentId { get; private set; }

        /// <summary>
        /// Id основного контента (AbstractItem)
        /// </summary>
        public int BaseContentId { get; private set; }

        public void AttachTo(Type type, string propertyName, int contentId, int baseContentId)
        {
            Type = type;
            PropertyName = propertyName;
            ContentId = contentId;
            BaseContentId = baseContentId;
        }

        public string Process(IServiceProvider serviceProvider, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                var qpUrlResolver = serviceProvider.GetService<IQpUrlResolver>();
                var qpSettings = serviceProvider.GetService<QpSettings>();
                //сначала пытаемся найти св-во в контенте-расширении, потом если не нашли в основном контенте
                var baseUrl = qpUrlResolver.UrlForImage(qpSettings.SiteId, ContentId, PropertyName);
                if (baseUrl == null)
                    baseUrl = qpUrlResolver.UrlForImage(qpSettings.SiteId, BaseContentId, PropertyName);
                if (!String.IsNullOrEmpty(baseUrl))
                {
                    return baseUrl + "/" + value;
                }
            }
            return value;
        }
    }
}

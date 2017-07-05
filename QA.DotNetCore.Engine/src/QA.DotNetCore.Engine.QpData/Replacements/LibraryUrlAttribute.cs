using System;
using Microsoft.Extensions.DependencyInjection;

namespace QA.DotNetCore.Engine.QpData.Replacements
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class LibraryUrlAttribute : Attribute, ILoaderOption
    {
        public Type Type { get; private set; }

        public string PropertyName { get; private set; }

        public int ContentId { get; private set; }

        public void AttachTo(Type type, string propertyName, int contentId)
        {
            Type = type;
            PropertyName = propertyName;
            ContentId = contentId;
        }

        public string Process(IServiceProvider serviceProvider, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                var qpUrlResolver = serviceProvider.GetService<IQpUrlResolver>();
                var baseUrl = qpUrlResolver.UrlForImage(ContentId, PropertyName);
                if (!String.IsNullOrEmpty(baseUrl))
                {
                    return baseUrl + "/" + value;
                }
            }
            return value;
        }
    }
}

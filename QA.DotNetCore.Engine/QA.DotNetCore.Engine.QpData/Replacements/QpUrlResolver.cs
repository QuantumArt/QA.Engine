using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.QpData.Replacements
{
    public class QpUrlResolver : IQpUrlResolver
    {
        ICacheProvider _cacheProvider;
        IMetaInfoRepository _metaInfoRepository;
        QpSettings _qpSettings;

        public QpUrlResolver(
            ICacheProvider cacheProvider,
            IMetaInfoRepository metaInfoRepository,
            IOptions<QpSettings> qpSettings)
        {
            _cacheProvider = cacheProvider;
            _metaInfoRepository = metaInfoRepository;
            _qpSettings = qpSettings.Value;
        }

        public string UploadUrl(bool removeScheme = false)
        {
            return LibraryUrl(removeScheme).TrimEnd('/') + "/images";
        }

        public string UrlForImage(int contentId, string fieldName, bool removeScheme = false)
        {
            var attr = GetContentAttribute(contentId, fieldName);
            if (attr == null)
                return null;

            var baseUrl = new StringBuilder();
            baseUrl.Append(LibraryUrl(removeScheme));
            if (!attr.UseSiteLibrary)
            {
                if (baseUrl[baseUrl.Length - 1] != '/')
                {
                    baseUrl.Append("/");
                }
                baseUrl.Append("contents/");
                baseUrl.Append(contentId);
            }

            return CombineWithoutDoubleSlashes(baseUrl.ToString(), attr.SubFolder?.Replace(@"\", @"/"));
        }

        private string LibraryUrl(bool removeScheme)
        {
            var site = GetSite();
            if (site == null)
                return null;

            var sb = new StringBuilder();
            var prefix = site.UseAbsoluteUploadUrl ? site.UploadUrlPrefix : string.Empty;
            if (!string.IsNullOrEmpty(prefix))
            {
                if (removeScheme)
                {
                    prefix = ConvertUrlToSchemaInvariant(prefix);
                }

                sb.Append(prefix);
            }
            else
            {
                sb.Append(!removeScheme ? "http://" : "//");
                sb.Append(site.Dns);
            }
            sb.Append(site.UploadUrl);

            return sb.ToString();
        }

        private QpSitePersistentData GetSite()
        {
            var cacheKey = "QpUrlResolver.GetSite";
            var result = _cacheProvider.Get(cacheKey) as QpSitePersistentData;
            if (result == null)
            {
                result = _metaInfoRepository.GetSite(_qpSettings.SiteId);
                if (result != null)
                {
                    _cacheProvider.Set(cacheKey, result, _qpSettings.CachePeriod);
                }
            }
            return result;
        }

        private ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName)
        {
            var cacheKey = $"QpUrlResolver.GetContentAttribute_{contentId}_{fieldName}";
            var result = _cacheProvider.Get(cacheKey) as ContentAttributePersistentData;
            if (result == null)
            {
                result = _metaInfoRepository.GetContentAttribute(contentId, fieldName);
                if (result != null)
                {
                    _cacheProvider.Set(cacheKey, result, _qpSettings.CachePeriod);
                }
            }
            return result;
        }

        private static string ConvertUrlToSchemaInvariant(string prefix)
        {
            if (prefix.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                prefix = "//" + prefix.Substring(7);
            }
            return prefix;
        }

        private static string CombineWithoutDoubleSlashes(string first, string second)
        {
            if (string.IsNullOrEmpty(second))
            {
                return first;
            }

            var sb = new StringBuilder();
            sb.Append(first.Replace(@":/", @"://").TrimEnd('/'));
            sb.Append("/");
            sb.Append(second.Replace("//", "/").TrimStart('/'));

            return sb.ToString();
        }
    }
}

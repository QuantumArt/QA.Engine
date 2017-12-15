using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;
using Microsoft.Extensions.Options;
using System.Text;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.QpData.Replacements
{
    /// <summary>
    /// Правила построения урлов до файлов медиа-библиотеки QP
    /// </summary>
    public class QpUrlResolver : IQpUrlResolver
    {
        ICacheProvider _cacheProvider;
        IMetaInfoRepository _metaInfoRepository;
        QpSettings _qpSettings;
        QpSchemeCacheSettings _qpSchemeSettings;

        public QpUrlResolver(
            ICacheProvider cacheProvider,
            IMetaInfoRepository metaInfoRepository,
            QpSettings qpSettings,
            QpSchemeCacheSettings qpSchemeSettings)
        {
            _cacheProvider = cacheProvider;
            _metaInfoRepository = metaInfoRepository;
            _qpSettings = qpSettings;
            _qpSchemeSettings = qpSchemeSettings;
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
            return _cacheProvider.GetOrAdd("QpUrlResolver.GetSite", _qpSchemeSettings.CachePeriod, () => _metaInfoRepository.GetSite(_qpSettings.SiteId));
        }

        private ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName)
        {
            return _cacheProvider.GetOrAdd($"QpUrlResolver.GetContentAttribute_{contentId}_{fieldName}", _qpSchemeSettings.CachePeriod, () => _metaInfoRepository.GetContentAttribute(contentId, fieldName));
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

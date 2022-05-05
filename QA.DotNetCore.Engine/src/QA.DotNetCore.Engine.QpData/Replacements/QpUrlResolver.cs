using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Text;

namespace QA.DotNetCore.Engine.QpData.Replacements
{
    /// <summary>
    /// Правила построения урлов до файлов медиа-библиотеки QP
    /// </summary>
    public class QpUrlResolver : IQpUrlResolver
    {
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private readonly IMetaInfoRepository _metaInfoRepository;
        private readonly QpSiteStructureCacheSettings _qpSchemeSettings;

        public QpUrlResolver(
            IMemoryCacheProvider memoryCacheProvider,
            IMetaInfoRepository metaInfoRepository,
            QpSiteStructureCacheSettings qpSchemeSettings)
        {
            _memoryCacheProvider = memoryCacheProvider;
            _metaInfoRepository = metaInfoRepository;
            _qpSchemeSettings = qpSchemeSettings;
        }

        public string UploadUrl(int siteId, bool removeScheme = false)
        {
            return LibraryUrl(siteId, removeScheme).TrimEnd('/') + "/images";
        }

        public string UrlForImage(int siteId, int contentId, string fieldName, bool removeScheme = false)
        {
            var attr = GetContentAttribute(contentId, fieldName);
            return UrlForImage(siteId, attr, removeScheme);
        }

        public string UrlForImage(int siteId, ContentAttributePersistentData attr, bool removeScheme = false)
        {
            if (attr == null)
                return null;

            var baseUrl = new StringBuilder();
            if (attr.UseSiteLibrary)
            {
                baseUrl.Append(UploadUrl(siteId, removeScheme));
            }
            else
            {
                baseUrl.Append(LibraryUrl(siteId, removeScheme));
                if (baseUrl[baseUrl.Length - 1] != '/')
                {
                    baseUrl.Append("/");
                }
                baseUrl.Append("contents/");
                baseUrl.Append(attr.ContentId);
            }

            return CombineWithoutDoubleSlashes(baseUrl.ToString(), attr.SubFolder?.Replace(@"\", @"/"));
        }

        private string LibraryUrl(int siteId, bool removeScheme)
        {
            var site = GetSite(siteId);
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

                sb.Append(prefix.TrimEnd('/'));
            }
            else
            {
                sb.Append(!removeScheme ? "http://" : "//");
                sb.Append(site.Dns.TrimEnd('/'));
            }

            sb.Append(site.UploadUrl);

            return sb.ToString();
        }

        private QpSitePersistentData GetSite(int siteId)
        {
            return _memoryCacheProvider.GetOrAdd(
                $"QpUrlResolver.GetSite{siteId}",
                _qpSchemeSettings.QpSchemeCachePeriod,
                () => _metaInfoRepository.GetSite(siteId));
        }

        private ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName)
        {
            return _memoryCacheProvider.GetOrAdd(
                $"QpUrlResolver.GetContentAttribute_{contentId}_{fieldName.ToUpper()}",
                _qpSchemeSettings.QpSchemeCachePeriod,
                () => _metaInfoRepository.GetContentAttribute(contentId, fieldName));
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
            sb.Append(first.TrimEnd('/'));
            sb.Append("/");
            sb.Append(second.Replace("//", "/").TrimStart('/'));

            return sb.ToString();
        }
    }
}

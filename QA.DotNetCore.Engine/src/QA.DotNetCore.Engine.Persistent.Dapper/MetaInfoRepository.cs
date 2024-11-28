using Dapper;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NLog;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class MetaInfoRepository : IMetaInfoRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly IServiceProvider _serviceProvider;
        private IUnitOfWork _unitOfWork;

        public MetaInfoRepository(
            IServiceProvider serviceProvider,
            IMemoryCacheProvider memoryCacheProvider,
            QpSiteStructureCacheSettings cacheSettings)
        {
            _serviceProvider = serviceProvider;
            _memoryCacheProvider = memoryCacheProvider;
            _cacheSettings = cacheSettings;
        }

        protected IUnitOfWork UnitOfWork {
            get
            {
                if (_unitOfWork != null)
                {
                    _logger.ForTraceEvent()
                        .Message($"Using existing UnitOfWork {_unitOfWork.Id}")
                        .Log();
                    return _unitOfWork;
                }
                var uow = _serviceProvider.GetRequiredService<IUnitOfWork>();
                _logger.ForTraceEvent()
                    .Message($"Received UnitOfWork {uow.Id} from ServiceProvider")
                    .Log();
                return uow;
            }
        }

        private const string CmdGetSite = @"
SELECT
    USE_ABSOLUTE_UPLOAD_URL as UseAbsoluteUploadUrl,
    UPLOAD_URL_PREFIX as UploadUrlPrefix,
    UPLOAD_URL as UploadUrl,
    DNS
FROM SITE
WHERE SITE_ID = {0}";

        private const string CmdGetContentAttributeByName = @"
SELECT
    ca.ATTRIBUTE_ID as Id,
    ca.CONTENT_ID as ContentId,
    ca.ATTRIBUTE_NAME as ColumnName,
    ca.NET_ATTRIBUTE_NAME as NetName,
    ca.USE_SITE_LIBRARY as UseSiteLibrary,
    ca.SUBFOLDER,
    at.TYPE_NAME as TypeName,
    ca.LINK_ID as M2MLinkId
FROM CONTENT_ATTRIBUTE ca
INNER JOIN ATTRIBUTE_TYPE at ON at.ATTRIBUTE_TYPE_ID=ca.ATTRIBUTE_TYPE_ID
WHERE ca.CONTENT_ID={0} AND lower(ca.ATTRIBUTE_NAME)=lower('{1}')
";

        private const string CmdGetContentAttributeByNetName = @"
SELECT
    ca.ATTRIBUTE_ID as Id,
    ca.CONTENT_ID as ContentId,
    ca.ATTRIBUTE_NAME as ColumnName,
    ca.NET_ATTRIBUTE_NAME as NetName,
    ca.USE_SITE_LIBRARY as UseSiteLibrary,
    ca.SUBFOLDER,
    at.TYPE_NAME as TypeName,
    ca.LINK_ID as M2MLinkId
FROM CONTENT_ATTRIBUTE ca
INNER JOIN ATTRIBUTE_TYPE at ON at.ATTRIBUTE_TYPE_ID=ca.ATTRIBUTE_TYPE_ID
WHERE ca.CONTENT_ID={0} AND lower(ca.NET_ATTRIBUTE_NAME)=lower('{1}')
";

        private const string BaseCmdGetContents = @"
SELECT
    c.CONTENT_NAME as " + nameof(ContentPersistentData.ContentName) + @",
    c.SITE_ID as " + nameof(ContentPersistentData.SiteId) + @",
    c.NET_CONTENT_NAME as " + nameof(ContentPersistentData.ContentNetName) + @",
    c.USE_DEFAULT_FILTRATION as " + nameof(ContentAttributePersistentData.UseDefaultFiltration) + @",
    ca.ATTRIBUTE_ID as " + nameof(ContentAttributePersistentData.Id) + @",
    ca.CONTENT_ID as " + nameof(ContentAttributePersistentData.ContentId) + @",
    ca.ATTRIBUTE_NAME as " + nameof(ContentAttributePersistentData.ColumnName) + @",
    ca.NET_ATTRIBUTE_NAME as " + nameof(ContentAttributePersistentData.NetName) + @",
    ca.USE_SITE_LIBRARY as " + nameof(ContentAttributePersistentData.UseSiteLibrary) + @",
    ca.SUBFOLDER as " + nameof(ContentAttributePersistentData.SubFolder) + @",
    at.TYPE_NAME as " + nameof(ContentAttributePersistentData.TypeName) + @",
    ca.LINK_ID as " + nameof(ContentAttributePersistentData.M2MLinkId) + @"
FROM CONTENT c
INNER JOIN CONTENT_ATTRIBUTE ca on ca.CONTENT_ID = c.CONTENT_ID
INNER JOIN ATTRIBUTE_TYPE at ON at.ATTRIBUTE_TYPE_ID = ca.ATTRIBUTE_TYPE_ID
";

        private const string CmdGetContentsByNetName = BaseCmdGetContents + "WHERE c.SITE_ID = {0} AND lower(c.NET_CONTENT_NAME) IN ({1})";
        private const string CmdGetContentsById = BaseCmdGetContents + " WHERE c.CONTENT_ID in ({1})";

        public QpSitePersistentData GetSite(int siteId)
        {
            return GetSite(siteId, null);
        }

        public QpSitePersistentData GetSite(int siteId, IDbTransaction transaction = null)
        {
            return UnitOfWork.Connection.QueryFirst<QpSitePersistentData>(string.Format(CmdGetSite, siteId), transaction: transaction);
        }

        public ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName)
        {
            return GetContentAttribute(contentId, fieldName, null);
        }

        public ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName, IDbTransaction transaction = null)
        {
            return UnitOfWork.Connection.QueryFirstOrDefault<ContentAttributePersistentData>(
                string.Format(CmdGetContentAttributeByName, contentId, fieldName), transaction: transaction);
        }

        public ContentAttributePersistentData GetContentAttributeByNetName(int contentId, string fieldNetName, IDbTransaction transaction = null)
        {
            return UnitOfWork.Connection.QueryFirstOrDefault<ContentAttributePersistentData>(
                string.Format(CmdGetContentAttributeByNetName, contentId, fieldNetName), transaction: transaction);
        }

        public ContentPersistentData[] GetContents(ICollection<string> contentNetNames, int siteId, IDbTransaction transaction = null)
        {
            string[] normalizedNames = contentNetNames.Select(name => name.ToLower()).ToArray();
            return GetContentsCore(
                nameof(CmdGetContentsByNetName),
                CmdGetContentsByNetName,
                normalizedNames,
                siteId,
                transaction);
        }

        public ContentPersistentData GetContent(string contentNetName, int siteId, IDbTransaction transaction = null) =>
            GetContents(new[] { contentNetName }, siteId, transaction).FirstOrDefault();

        public ContentPersistentData[] GetContentsById(int[] contentIds, IDbTransaction transaction = null) =>
            GetContentsCore(
                nameof(CmdGetContentsById),
                CmdGetContentsById,
                contentIds,
                0,
                transaction);

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private ContentPersistentData[] GetContentsCore<T>(
            string templateId,
            string queryTemplate,
            ICollection<T> parameterValues,
            int siteId,
            IDbTransaction transaction = null)
        {
            Debug.Assert(queryTemplate != null);

            if (parameterValues is null)
                throw new ArgumentNullException(nameof(parameterValues));

            if (!parameterValues.Any())
                throw new ArgumentException("Argument must be non-empty collection.", nameof(parameterValues));

            var (queryFragment, parameters) = GetParametersInfo(parameterValues);

            string query = string.Format(queryTemplate, siteId, queryFragment);

            // TODO: Add support for array of keys to request a batch from redis for more persistent cache. Consider adding lazy interface.
            string parametersList = string.Join("|", parameterValues.OrderBy(value => value));
            string cacheKey = $"{nameof(GetContentsCore)}_{templateId}_{parametersList}_{siteId}";
            TimeSpan expiry = _cacheSettings.QpSchemeCachePeriod;

            var attributes = _memoryCacheProvider.GetOrAdd(
                cacheKey,
                expiry,
                () => UnitOfWork.Connection.Query<ContentAttributePersistentData>(query, parameters, transaction));

            return GroupAttributesIntoContents(attributes).ToArray();
        }

        private static (string queryFragment, Dictionary<string, object> parameters) GetParametersInfo<T>(
            ICollection<T> parameterValues)
        {
            if (parameterValues is null)
                throw new ArgumentNullException(nameof(parameterValues));

            if (!parameterValues.Any())
                throw new ArgumentException("Argument must be non-empty collection.", nameof(parameterValues));

            const int maximumExpectedParameterNameSize = 6;

            var parametersQuery = new StringBuilder(parameterValues.Count * maximumExpectedParameterNameSize);
            var parameters = new Dictionary<string, object>();
            using (IEnumerator<T> namesEnumerator = parameterValues.GetEnumerator())
            {
                for (int i = 0; namesEnumerator.MoveNext(); i++)
                {
                    var parameterName = $"@t{i}";
                    _ = parametersQuery.Append(parameterName).Append(',');
                    parameters.Add(parameterName, namesEnumerator.Current);
                }
                // Removing trailing comma.
                if (parametersQuery.Length > 0)
                    parametersQuery.Length--;
            }

            return (parametersQuery.ToString(), parameters);
        }

        private static IEnumerable<ContentPersistentData> GroupAttributesIntoContents(
            IEnumerable<ContentAttributePersistentData> attributes)
        {
            return attributes
                .GroupBy(
                    attribute => (attribute.ContentId, attribute.ContentName, attribute.SiteId, attribute.ContentNetName),
                    attribute => attribute,
                    (key, value) => new ContentPersistentData
                    {
                        ContentId = key.ContentId,
                        ContentName = key.ContentName,
                        ContentNetName = key.ContentNetName,
                        SiteId = key.SiteId,
                        ContentAttributes = value
                    });
        }
    }
}

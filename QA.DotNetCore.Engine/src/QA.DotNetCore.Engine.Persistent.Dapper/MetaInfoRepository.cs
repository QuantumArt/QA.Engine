using Dapper;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class MetaInfoRepository : IMetaInfoRepository
    {
        private readonly IServiceProvider _serviceProvider;

        public MetaInfoRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected IUnitOfWork UnitOfWork { get { return _serviceProvider.GetRequiredService<IUnitOfWork>(); } }

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
    ca.LINK_ID as M2mLinkId
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
    ca.LINK_ID as M2mLinkId
FROM CONTENT_ATTRIBUTE ca
INNER JOIN ATTRIBUTE_TYPE at ON at.ATTRIBUTE_TYPE_ID=ca.ATTRIBUTE_TYPE_ID
WHERE ca.CONTENT_ID={0} AND lower(ca.NET_ATTRIBUTE_NAME)=lower('{1}')
";

        private const string CmdGetContents = @"
SELECT
    c.CONTENT_NAME as " + nameof(ContentPersistentData.ContentName) + @",
    c.NET_CONTENT_NAME as " + nameof(ContentPersistentData.ContentNetName) + @",
    c.USE_DEFAULT_FILTRATION as " + nameof(ContentAttributePersistentData.UseDefaultFiltration) + @",
    ca.ATTRIBUTE_ID as " + nameof(ContentAttributePersistentData.Id) + @",
    ca.CONTENT_ID as " + nameof(ContentAttributePersistentData.ContentId) + @",
    ca.ATTRIBUTE_NAME as " + nameof(ContentAttributePersistentData.ColumnName) + @",
    ca.NET_ATTRIBUTE_NAME as " + nameof(ContentAttributePersistentData.NetName) + @",
    ca.USE_SITE_LIBRARY as " + nameof(ContentAttributePersistentData.UseSiteLibrary) + @",
    ca.SUBFOLDER as " + nameof(ContentAttributePersistentData.SubFolder) + @",
    at.TYPE_NAME as " + nameof(ContentAttributePersistentData.TypeName) + @",
    ca.LINK_ID as " + nameof(ContentAttributePersistentData.M2mLinkId) + @"
FROM CONTENT c
INNER JOIN CONTENT_ATTRIBUTE ca on ca.CONTENT_ID = c.CONTENT_ID
INNER JOIN ATTRIBUTE_TYPE at ON at.ATTRIBUTE_TYPE_ID = ca.ATTRIBUTE_TYPE_ID
WHERE c.SITE_ID = {0} AND lower(c.NET_CONTENT_NAME) IN ({1})";

        private const string CmdGetContentsById = @"
SELECT
    c.CONTENT_NAME as ContentName,
    c.NET_CONTENT_NAME as ContentNetName,
    c.USE_DEFAULT_FILTRATION as UseDefaultFiltration,
    ca.ATTRIBUTE_ID as Id,
    ca.CONTENT_ID as ContentId,
    ca.ATTRIBUTE_NAME as ColumnName,
    ca.NET_ATTRIBUTE_NAME as NetName,
    ca.USE_SITE_LIBRARY as UseSiteLibrary,
    ca.SUBFOLDER,
    at.TYPE_NAME as TypeName,
    ca.LINK_ID as M2mLinkId
FROM CONTENT c
INNER JOIN CONTENT_ATTRIBUTE ca on ca.CONTENT_ID = c.CONTENT_ID
INNER JOIN ATTRIBUTE_TYPE at ON at.ATTRIBUTE_TYPE_ID=ca.ATTRIBUTE_TYPE_ID
WHERE c.SITE_ID={0} AND c.CONTENT_ID in ({1})
";

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
            if (contentNetNames is null)
                throw new ArgumentNullException(nameof(contentNetNames));

            if (!contentNetNames.Any())
                throw new ArgumentException("Argument must be non-empty collection.", nameof(contentNetNames));

            const int maximumExpectedParameterNameSize = 6;
            var parametersQuery = new StringBuilder(contentNetNames.Count * maximumExpectedParameterNameSize);
            var parameters = new Dictionary<string, object>();

            using (IEnumerator<string> namesEnumerator = contentNetNames.GetEnumerator())
            {
                for (int i = 0; namesEnumerator.MoveNext(); i++)
                {
                    var parameterName = $"@t{i}";
                    _ = parametersQuery.Append(parameterName).Append(',');
                    parameters.Add(parameterName, namesEnumerator.Current.ToLower());
                }
                // Removing trailing comma.
                if (parametersQuery.Length > 0)
                    parametersQuery.Length--;
            }

            string query = string.Format(CmdGetContents, siteId, parametersQuery.ToString());

            var attributes = UnitOfWork.Connection
                .Query<ContentAttributePersistentData>(query, parameters, transaction);

            return attributes
                .GroupBy(
                    attribute => (attribute.ContentId, attribute.ContentName, attribute.ContentNetName),
                    attribute => attribute,
                    (key, value) => new ContentPersistentData
                    {
                        ContentId = key.ContentId,
                        ContentName = key.ContentName,
                        ContentNetName = key.ContentNetName,
                        ContentAttributes = attributes
                    })
                .ToArray();
        }

        public ContentPersistentData GetContent(string contentNetName, int siteId, IDbTransaction transaction = null) =>
            GetContents(new[] { contentNetName }, siteId, transaction).FirstOrDefault();

        public ContentPersistentData[] GetContentsById(int[] contentIds, int siteId, IDbTransaction transaction = null)
        {
            if (contentIds == null || contentIds.Length == 0)
                return new ContentPersistentData[0];

            string query = string.Format(CmdGetContentsById, siteId, string.Join(",", contentIds));
            var contentAttributes = UnitOfWork.Connection
                .Query<ContentAttributePersistentData>(query, transaction: transaction)
                .ToList();

            return contentAttributes
                .GroupBy(ca => ca.ContentId)
                .Select(g => new ContentPersistentData
                {
                    ContentId = g.Key,
                    ContentName = g.First().ContentName,
                    ContentNetName = g.First().ContentNetName,
                    ContentAttributes = g.ToArray()
                }).ToArray();
        }
    }
}

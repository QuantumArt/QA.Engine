using Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using System.Data;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class MetaInfoRepository : IMetaInfoRepository
    {
        private readonly IDbConnection _connection;

        public MetaInfoRepository(IUnitOfWork uow)
        {
            _connection = uow.Connection;
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

        private const string CmdGetContent = @"
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
WHERE c.SITE_ID={0} AND lower(c.NET_CONTENT_NAME)=lower('{1}')
";

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
            return _connection.QueryFirst<QpSitePersistentData>(string.Format(CmdGetSite, siteId), transaction: transaction);
        }

        public ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName)
        {
            return GetContentAttribute(contentId, fieldName, null);
        }

        public ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName, IDbTransaction transaction = null)
        {
            return _connection.QueryFirstOrDefault<ContentAttributePersistentData>(
                string.Format(CmdGetContentAttributeByName, contentId, fieldName), transaction: transaction);
        }

        public ContentAttributePersistentData GetContentAttributeByNetName(int contentId, string fieldNetName, IDbTransaction transaction = null)
        {
            return _connection.QueryFirstOrDefault<ContentAttributePersistentData>(
                string.Format(CmdGetContentAttributeByNetName, contentId, fieldNetName), transaction: transaction);
        }

        public ContentPersistentData GetContent(string contentNetName, int siteId, IDbTransaction transaction = null)
        {
            string query = string.Format(CmdGetContent, siteId, contentNetName);
            var contentAttributes = _connection
                .Query<ContentAttributePersistentData>(query, transaction: transaction)
                .ToList();
            if (contentAttributes == null || !contentAttributes.Any())
                return null;
            var contentAttribute = contentAttributes.First();
            return new ContentPersistentData
            {
                ContentId = contentAttribute.ContentId,
                ContentName = contentAttribute.ContentName,
                ContentNetName = contentNetName,
                ContentAttributes = contentAttributes
            };
        }

        public ContentPersistentData[] GetContentsById(int[] contentIds, int siteId, IDbTransaction transaction = null)
        {
            if (contentIds == null || contentIds.Length == 0)
                return new ContentPersistentData[0];

            string query = string.Format(CmdGetContentsById, siteId, string.Join(",", contentIds));
            var contentAttributes = _connection
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

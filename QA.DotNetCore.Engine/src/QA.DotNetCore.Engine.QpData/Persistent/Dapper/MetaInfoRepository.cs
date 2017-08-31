using Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
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
FROM [SITE]
WHERE SITE_ID = {0}";

        private const string CmdGetContentAttribute = @"
SELECT
    ATTRIBUTE_ID as Id,
    CONTENT_ID as ContentId,
    ATTRIBUTE_NAME as ColumnName,
    NET_ATTRIBUTE_NAME as NetName,
    USE_SITE_LIBRARY as UseSiteLibrary,
    SUBFOLDER
FROM [CONTENT_ATTRIBUTE]
WHERE CONTENT_ID={0} AND ATTRIBUTE_NAME='{1}'
";

        private const string CmdGetContent = @"
SELECT
    ca.ATTRIBUTE_ID as Id,
    ca.CONTENT_ID as ContentId,
    ca.ATTRIBUTE_NAME as ColumnName,
    ca.NET_ATTRIBUTE_NAME as NetName,
    ca.USE_SITE_LIBRARY as UseSiteLibrary,
    ca.SUBFOLDER
FROM [CONTENT] c
INNER JOIN [CONTENT_ATTRIBUTE] ca on ca.CONTENT_ID = c.CONTENT_ID
WHERE c.[SITE_ID]={0} AND c.[NET_CONTENT_NAME]='{1}'
";

        public QpSitePersistentData GetSite(int siteId)
        {
            return _connection.QueryFirst<QpSitePersistentData>(string.Format(CmdGetSite, siteId));
        }

        public ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName)
        {
            return _connection.QueryFirstOrDefault<ContentAttributePersistentData>(string.Format(CmdGetContentAttribute, contentId, fieldName));
        }

        public ContentPersistentData GetContent(string contentNetName, int siteId)
        {
            var contentAttributes = _connection.Query<ContentAttributePersistentData>(string.Format(CmdGetContent, siteId, contentNetName)).ToList();
            if (contentAttributes == null || !contentAttributes.Any())
                return null;

            return new ContentPersistentData
            {
                ContentId = contentAttributes.First().ContentId,
                ContentNetName = contentNetName,
                ContentAttributes = contentAttributes
            };
        }
    }
}

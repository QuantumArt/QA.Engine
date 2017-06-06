using Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using System.Data;

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
    USE_SITE_LIBRARY as UseSiteLibrary,
    SUBFOLDER
FROM [CONTENT_ATTRIBUTE]
WHERE CONTENT_ID={0} AND ATTRIBUTE_NAME='{1}'
";

        public QpSitePersistentData GetSite(int siteId)
        {
            return _connection.QueryFirst<QpSitePersistentData>(string.Format(CmdGetSite, siteId));
        }

        public ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName)
        {
            return _connection.QueryFirst<ContentAttributePersistentData>(string.Format(CmdGetContentAttribute, contentId, fieldName));
        }
    }
}

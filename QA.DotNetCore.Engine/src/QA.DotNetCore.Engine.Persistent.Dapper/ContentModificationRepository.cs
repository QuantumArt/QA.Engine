using Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using System.Data;

namespace QA.DotNetCore.Engine.Persistent.Dapper
{
    public class ContentModificationRepository : IContentModificationRepository
    {
        private readonly IDbConnection _connection;

        public ContentModificationRepository(IUnitOfWork uow)
        {
            _connection = uow.Connection;
        }

        private const string CmdGetAll = @"
SELECT
	c.CONTENT_ID as ContentId,
	c.CONTENT_NAME as ContentName,
    c.SITE_ID as SiteId,
	cm.LIVE_MODIFIED as LiveModified,
	cm.STAGE_MODIFIED as StageModified
FROM CONTENT_MODIFICATION cm
INNER JOIN CONTENT c on c.CONTENT_ID = cm.CONTENT_ID";

        public IEnumerable<QpContentModificationPersistentData> GetAll(IDbTransaction transaction = null)
        {
            return _connection.Query<QpContentModificationPersistentData>(CmdGetAll, transaction);
        }
    }
}

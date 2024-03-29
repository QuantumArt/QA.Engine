using Dapper;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace QA.DotNetCore.Engine.Persistent.Dapper
{
    public class ContentModificationRepository : IContentModificationRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private IUnitOfWork _unitOfWork;

        private const string CmdGetAll = @"
SELECT c.CONTENT_ID as ContentId, c.CONTENT_NAME as ContentName, c.SITE_ID as SiteId,
    cm.LIVE_MODIFIED as LiveModified, cm.STAGE_MODIFIED as StageModified
FROM CONTENT_MODIFICATION cm
INNER JOIN CONTENT c on c.CONTENT_ID = cm.CONTENT_ID";

        public ContentModificationRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected IUnitOfWork UnitOfWork => _unitOfWork ?? _serviceProvider.GetRequiredService<IUnitOfWork>();

        public void SetUnitOfWork(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;



        public IEnumerable<QpContentModificationPersistentData> GetAll(IDbTransaction transaction = null)
        {
            return UnitOfWork.Connection.Query<QpContentModificationPersistentData>(CmdGetAll, transaction);
        }
    }
}

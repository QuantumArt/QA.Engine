using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces.Logging;

namespace QA.DotNetCore.Engine.Persistent.Dapper
{
    public class ContentModificationRepository : IContentModificationRepository
    {
        private readonly ILogger _logger;
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
            _logger = serviceProvider.GetService<ILogger<ContentModificationRepository>>();
        }

        protected IUnitOfWork UnitOfWork {
            get
            {
                if (_unitOfWork == null)
                {
                    _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
                    _logger.BeginScopeWith(("unitOfWorkId", _unitOfWork.Id));
                    _logger.LogTrace("Received UnitOfWork from ServiceProvider");
                }
                return _unitOfWork;
            }
        }

        public void SetUnitOfWork(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IEnumerable<QpContentModificationPersistentData> GetAll(IDbTransaction transaction = null)
        {
            _logger.BeginScopeWith(("unitOfWorkId", UnitOfWork.Id));
            _logger.LogTrace("Received content modifications");
            return UnitOfWork.Connection.Query<QpContentModificationPersistentData>(CmdGetAll, transaction);
        }
    }
}

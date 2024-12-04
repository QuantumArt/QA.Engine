using Dapper;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Data;
using NLog;

namespace QA.DotNetCore.Engine.Persistent.Dapper
{
    public class ContentModificationRepository : IContentModificationRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
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

        protected IUnitOfWork UnitOfWork {
            get
            {
                if (_unitOfWork == null)
                {
                    _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
                    _logger.ForTraceEvent()
                        .Message("Received UnitOfWork from ServiceProvider")
                        .Property("unitOfWorkId", _unitOfWork.Id)
                        .Log();
                }
                return _unitOfWork;
            }
        }

        public void SetUnitOfWork(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IEnumerable<QpContentModificationPersistentData> GetAll(IDbTransaction transaction = null)
        {
            _logger.ForTraceEvent().Message("Received content modifications")
                .Property("unitOfWorkId", UnitOfWork.Id)
                .Log();

            return UnitOfWork.Connection.Query<QpContentModificationPersistentData>(CmdGetAll, transaction);
        }
    }
}

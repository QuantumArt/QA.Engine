using System;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace QA.DotNetCore.Engine.CacheTags
{
    /// <summary>
    /// Реализация ICacheTagTracker для контентов QP.
    /// </summary>
    public class QpContentCacheTracker : ICacheTagTracker
    {
        private readonly IContentModificationRepository _contentModificationRepository;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly Func<IServiceProvider, IUnitOfWork> _unitOfWorkFunc;
        private readonly IServiceScopeFactory _scopeFactory;
        public QpContentCacheTracker(IContentModificationRepository contentModificationRepository,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            Func<IServiceProvider, IUnitOfWork> unitOfWorkFunc,
            IServiceScopeFactory scopeFactory
            )
        {
            _contentModificationRepository = contentModificationRepository;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _unitOfWorkFunc = unitOfWorkFunc;
            _scopeFactory = scopeFactory;
        }

        public IEnumerable<CacheTagModification> TrackChanges()
        {
            using var scope = _scopeFactory.CreateScope();
            using var unitOfWork = _unitOfWorkFunc(scope.ServiceProvider);

            _contentModificationRepository.SetUnitOfWork(unitOfWork);
            _qpContentCacheTagNamingProvider.SetUnitOfWork(unitOfWork);
            var result = new List<CacheTagModification>();
            foreach (var modification in _contentModificationRepository.GetAll())
            {
                result.Add(new CacheTagModification(CacheTagName(modification, isStage: true), modification.StageModified));
                result.Add(new CacheTagModification(CacheTagName(modification, isStage: false), modification.LiveModified));
            }
            return result;
        }

        private string CacheTagName(QpContentModificationPersistentData modification, bool isStage)
        {
            return _qpContentCacheTagNamingProvider.Get(modification.ContentName, modification.SiteId, isStage);
        }
    }
}

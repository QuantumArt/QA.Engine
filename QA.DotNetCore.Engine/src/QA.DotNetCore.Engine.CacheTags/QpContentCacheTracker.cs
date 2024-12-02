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
        public QpContentCacheTracker(IContentModificationRepository contentModificationRepository,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider
            )
        {
            _contentModificationRepository = contentModificationRepository;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
        }

        public IEnumerable<CacheTagModification> TrackChanges(IServiceProvider provider)
        {
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
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

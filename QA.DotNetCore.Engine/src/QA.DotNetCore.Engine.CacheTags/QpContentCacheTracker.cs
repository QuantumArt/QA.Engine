using System;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.CacheTags
{
    /// <summary>
    /// Реализация ICacheTagTracker для контентов QP.
    /// </summary>
    public class QpContentCacheTracker : ICacheTagTracker
    {
        private readonly IContentModificationRepository _contentModificationRepository;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly Func<IUnitOfWork> _unitOfWorkFunc;
        public QpContentCacheTracker(IContentModificationRepository contentModificationRepository,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            Func<IUnitOfWork> unitOfWorkFunc
            )
        {
            _contentModificationRepository = contentModificationRepository;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _unitOfWorkFunc = unitOfWorkFunc;
        }

        public IEnumerable<CacheTagModification> TrackChanges()
        {
            using (var unitOfWork = _unitOfWorkFunc())
            {
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

        }

        private string CacheTagName(QpContentModificationPersistentData modification, bool isStage)
        {
            return _qpContentCacheTagNamingProvider.Get(modification.ContentName, modification.SiteId, isStage);
        }
    }
}

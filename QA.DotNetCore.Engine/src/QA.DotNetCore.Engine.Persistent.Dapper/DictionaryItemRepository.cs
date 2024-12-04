using Dapper;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NLog;

namespace QA.DotNetCore.Engine.Persistent.Dapper
{
    public class DictionaryItemRepository : IDictionaryItemRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly ICacheProvider _cacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;
        protected IUnitOfWork UnitOfWork
        {
            get
            {
                var uow = _serviceProvider.GetRequiredService<IUnitOfWork>();
                _logger.ForTraceEvent()
                    .Message("Received UnitOfWork from ServiceProvider")
                    .Property("unitOfWorkId", uow.Id)
                    .Log();
                return uow;
            }
        }

        public DictionaryItemRepository(
            IServiceProvider serviceProvider,
            INetNameQueryAnalyzer netNameQueryAnalyzer,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            ICacheProvider cacheProvider,
            QpSiteStructureCacheSettings cacheSettings)
        {
            _serviceProvider = serviceProvider;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
            _cacheSettings = cacheSettings;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
        }

        private string GetCmdGetAll(DictionarySettings settings) => @$"
            SELECT
                CONTENT_ITEM_ID as Id,
                {GetParentField(settings)} as ParentId,
                {GetAliasField(settings)} as Alias,
                |{settings.NetName}.{settings.TitleFieldName}| as Title
            FROM |{settings.NetName}|";


        private string GetAliasField(DictionarySettings settings)
        {
            if (settings.AliasFieldName == null || settings.AliasFieldName == "CONTENT_ITEM_ID")
            {
                return "CONTENT_ITEM_ID";
            }
            else
            {
                return $"|{settings.NetName}.{settings.AliasFieldName}|";
            }
        }

        private string GetParentField(DictionarySettings settings)
        {
            if (string.IsNullOrEmpty(settings.ParentIdFieldName))
            {
                return "null";
            }
            else
            {
                return $"|{settings.NetName}.{settings.ParentIdFieldName}|";
            }
        }

        public IEnumerable<DictionaryItemPersistentData> GetAllDictionaryItems(DictionarySettings settings, int siteId, bool isStage, IDbTransaction transaction = null)
        {
            var rawQuery = GetCmdGetAll(settings);
            var query = _netNameQueryAnalyzer.PrepareQuery(rawQuery, siteId, isStage);
            var cacheKey = query;
            var contentNetNames = _netNameQueryAnalyzer
                .GetContentNetNames(rawQuery, siteId, isStage)
                .ToArray();
            var cacheTags = _qpContentCacheTagNamingProvider
                .GetByContentNetNames(contentNetNames, siteId, isStage)
                .Select(n => n.Value)
                .ToArray();
            var expiry = _cacheSettings.ItemDefinitionCachePeriod;
            return _cacheProvider.GetOrAdd(
                cacheKey,
                cacheTags,
                expiry,
                () =>
                {
                    _logger.ForTraceEvent().Message("Get all dictionary items")
                        .Property("siteId", siteId)
                        .Property("isStage", isStage)
                        .Property("cacheKey", cacheKey)
                        .Property("cacheTags", cacheTags)
                        .Property("expiry", expiry)
                        .Property("unitOfWorkId", UnitOfWork.Id)
                        .Log();
                    return UnitOfWork.Connection.Query<DictionaryItemPersistentData>(query, transaction).ToList();
                });
        }
    }
}

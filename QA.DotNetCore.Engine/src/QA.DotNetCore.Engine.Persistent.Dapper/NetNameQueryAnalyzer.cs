using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class NetNameQueryAnalyzer : INetNameQueryAnalyzer
    {
        private static readonly Regex s_tokenRegex = new Regex(@"\|[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)?\|", RegexOptions.Compiled);

        private readonly QpSiteStructureCacheSettings _qpSchemeSettings;
        private readonly ICacheProvider _cacheProvider;
        private readonly IMetaInfoRepository _metaInfoRepository;

        public NetNameQueryAnalyzer(
            IMetaInfoRepository metaInfoRepository,
            ICacheProvider cacheProvider,
            QpSiteStructureCacheSettings qpSchemeSettings)
        {
            _metaInfoRepository = metaInfoRepository;
            _cacheProvider = cacheProvider;
            _qpSchemeSettings = qpSchemeSettings ?? throw new ArgumentNullException(nameof(qpSchemeSettings));
        }

        public IEnumerable<string> GetContentNetNames(string netNameQuery, int siteId, bool isStage, bool useUnited = false)
        {
            if (netNameQuery is null)
                throw new ArgumentNullException(nameof(netNameQuery));

            var contentNetNames = GetTableColumnPairs(netNameQuery)
                .Select(pair => pair.Table)
                .Distinct()
                .ToArray();
            ContentPersistentData[] contentsMetadata = GetContentsMetadata(contentNetNames, siteId);

            foreach (var metadata in contentsMetadata)
                yield return GetActualContentName(metadata, isStage, useUnited);
        }

        public string PrepareQuery(string netNameQuery, int siteId, bool isStage, bool useUnited = false)
        {
            var tableToColumnPairs = GetTableColumnPairs(netNameQuery).ToArray();

            var tableToColumnsDict = tableToColumnPairs
                .GroupBy(parts => parts.Table, parts => parts.Column, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Where(column => column != null));

            var contentsMetadata = GetContentsMetadata(tableToColumnsDict.Keys, siteId)
                .ToDictionary(metadata => metadata.ContentNetName, StringComparer.OrdinalIgnoreCase);

            if (tableToColumnsDict.Count != contentsMetadata.Count)
            {
                var missingTables = new HashSet<string>(tableToColumnsDict.Keys, StringComparer.OrdinalIgnoreCase);
                missingTables.SymmetricExceptWith(contentsMetadata.Keys);
                throw new Exception($"Some content netnames ('{string.Join("', '", missingTables)}') haven't been found for site {siteId}");
            }

            var replacements = new Dictionary<string, string>(tableToColumnPairs.Length, StringComparer.OrdinalIgnoreCase);
            foreach (var tableToColumnsGroup in tableToColumnsDict)
            {
                var tableNetName = tableToColumnsGroup.Key;
                Debug.Assert(contentsMetadata.ContainsKey(tableNetName), "Obtained metadata is inconsistent with requested names.");

                ContentPersistentData tableMetadata = contentsMetadata[tableNetName];
                string tableName = GetActualContentName(tableMetadata, isStage, useUnited);

                replacements.Add($"|{tableNetName}|", tableName);

                foreach (var columnNetName in tableToColumnsGroup.Value)
                {
                    ContentAttributePersistentData contentAttribute = tableMetadata.ContentAttributes
                        .FirstOrDefault(attribute => attribute.NetName == columnNetName);

                    if (contentAttribute is null)
                    {
                        throw new Exception($"Content attribute with netname '{columnNetName}' " +
                            $"haven't been found for table '{tableNetName}' and site {siteId}");
                    }

                    replacements.Add($"|{tableNetName}.{columnNetName}|", contentAttribute.ColumnName);
                }
            }

            return ReplaceTokens(netNameQuery, replacements);
        }

        private ContentPersistentData[] GetContentsMetadata(ICollection<string> contentNetNames, int siteId)
        {
            var cacheKey = $"CacheTagCache_{string.Join(",", contentNetNames)}_{siteId}";

            // Compatibility with single content name in DefaultQpContentCacheTagNamingProvider.
            if (contentNetNames.Count == 1)
            {
                var contentMetadata = _cacheProvider.GetOrAdd(
                    cacheKey,
                    _qpSchemeSettings.QpSchemeCachePeriod,
                    () => _metaInfoRepository.GetContent(contentNetNames.Single(), siteId));

                return contentMetadata is null
                    ? Array.Empty<ContentPersistentData>()
                    : new[] { contentMetadata };
            }

            return _cacheProvider.GetOrAdd(
                cacheKey,
                _qpSchemeSettings.QpSchemeCachePeriod,
                () => _metaInfoRepository.GetContents(contentNetNames, siteId));
        }

        /// <summary>
        /// Вычленяет из запроса токены с указанными netname таблиц и полей
        /// таблицы указываются как |tableNetName|
        /// столюцы указываются как |tableNetName.columnNetName|
        /// </summary>
        private IEnumerable<(string Table, string Column)> GetTableColumnPairs(string netNameQuery)
        {
            if (netNameQuery is null)
                throw new ArgumentNullException(nameof(netNameQuery));

            var tokens = s_tokenRegex.Matches(netNameQuery)
                   .Cast<Match>()
                   .Select(match => match.Value.Trim('|'))
                   .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var token in tokens)
            {
                string[] tableColumnPair = token.Split('.');

                yield return tableColumnPair.Length == 1
                    ? (tableColumnPair[0], null)
                    : (tableColumnPair[0], tableColumnPair[1]);
            }
        }

        private string GetActualContentName(ContentPersistentData tableMetadata, bool isStage, bool useUnited = false) =>
            useUnited ? tableMetadata.GetUnitedTableName() : tableMetadata.GetTableName(isStage);

        private string ReplaceTokens(string query, Dictionary<string, string> replacements) =>
            s_tokenRegex.Replace(query, match => replacements[match.Value]);
    }
}

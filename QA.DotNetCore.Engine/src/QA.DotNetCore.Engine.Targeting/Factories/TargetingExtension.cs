using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.Targeting.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Targeting.Factories
{
    public static class TargetingExtension
    {
        /// <summary>
        /// Добавляет <typeparamref name="RelationFilter"/> 
        /// </summary>
        /// <param name="currentTargeting">Параметры таргетинга</param>
        /// <param name="targetingKey">Ключ таргетинга</param>
        /// <param name="relationFieldName">Имя relation поля</param>
        /// <param name="logger">Логгер</param>
        /// <returns></returns>
        public static ITargetingFilter AddRelationFilter(
            this IDictionary<string, string> currentTargeting,
            string targetingKey,
            string relationFieldName,
            ILogger logger)
        {
            if (!currentTargeting.TryGetValue(targetingKey, out var targetingValue))
            {
                return new EmptyFilter();
            }

            var regionIds = GetGerionIds(targetingValue);

            if (regionIds.Count == 0)
            {
                return new EmptyFilter();
            }

            return new RelationFilter(item => item.GetRelationIds(relationFieldName), regionIds, logger);
        }

        /// <summary>
        /// Добавляет <typeparamref name="RelationFilter"/> 
        /// </summary>
        /// <param name="currentTargeting">Параметры таргетинга</param>
        /// <param name="targetingKey">Ключ таргетинга</param>
        /// <param name="relationSelector">Предикат выбора relation поля</param>
        /// <param name="logger">Логгер</param>
        /// <returns></returns>
        public static ITargetingFilter AddRelationFilter(
            this IDictionary<string, string> currentTargeting,
            string targetingKey,
            Func<AbstractItem, IEnumerable<int>> relationSelector,
            ILogger logger)
        {
            if (!currentTargeting.TryGetValue(targetingKey, out var targetingValue))
            {
                return new EmptyFilter();
            }

            var regionIds = GetGerionIds(targetingValue);

            if (regionIds.Count == 0)
            {
                return new EmptyFilter();
            }

            return new RelationFilter(relationSelector, regionIds, logger);
        }

        /// <summary>
        /// Добавляет <typeparamref name="RelationFilter"/> 
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="currentTargeting">Параметры таргетинга</param>
        /// <param name="targetingKey">Ключ таргетинга</param>
        /// <param name="relationSelector">Предикат выбора relation поля</param>
        /// <param name="logger">Логгер</param>
        /// <returns></returns>
        public static ITargetingFilter AddRelationFilter(
            this ITargetingFilter filter,
            IDictionary<string, string> currentTargeting,
            string targetingKey,
            Func<AbstractItem, IEnumerable<int>> relationSelector,
            ILogger logger) =>
            filter.AddFilter(currentTargeting.AddRelationFilter(targetingKey, relationSelector, logger));

        /// <summary>
        /// Добавляет <typeparamref name="RelationFilter"/> 
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="currentTargeting">Параметры таргетинга</param>
        /// <param name="targetingKey">Ключ таргетинга</param>
        /// <param name="fieldName">Имя relation поля</param>
        /// <param name="logger">Логгер</param>
        /// <returns></returns>
        public static ITargetingFilter AddRelationFilter(
            this ITargetingFilter filter,
            IDictionary<string, string> currentTargeting,
            string targetingKey,
            string fieldName,
            ILogger logger) =>
            filter.AddFilter(currentTargeting.AddRelationFilter(targetingKey, fieldName, logger));

        private static HashSet<int> GetGerionIds(string value) =>
            SplitTargetingRegions(value)
                .Distinct()
                .ToHashSet();

        private static IEnumerable<int> SplitTargetingRegions(string regionsString)
        {
            foreach (var regionPart in regionsString.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(regionPart, out int regionId))
                {
                    yield return regionId;
                }
            }
        }
    }
}

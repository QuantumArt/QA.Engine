using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.Widgets;
using QA.WidgetPlatform.Api.Application.Exceptions;
using QA.WidgetPlatform.Api.Models;
using QA.WidgetPlatform.Api.Services.Abstract;
using QA.WidgetPlatform.Api.TargetingFilters;

namespace QA.WidgetPlatform.Api.Services
{
    internal class SiteStructureService : ISiteStructureService
    {
        private readonly IAbstractItemStorageProvider _abstractItemStorageProvider;
        private readonly ITargetingFiltersFactory _targetingFiltersFactory;

        public SiteStructureService(IAbstractItemStorageProvider abstractItemStorageProvider,
            ITargetingFiltersFactory targetingFiltersFactory)
        {
            _abstractItemStorageProvider = abstractItemStorageProvider;
            _targetingFiltersFactory = targetingFiltersFactory;
        }

        /// <summary>
        /// Получение структуры страниц сайта
        /// </summary>
        /// <param name="dnsName">Доменное имя сайта</param>
        /// <param name="targeting">Словарь значений таргетирования</param>
        /// <param name="fields">Поля деталей к выдаче. Если пусто, то детали выдаваться не будут</param>
        /// <param name="deep">Глубина страуктуры, где 0 - это корневой элемент</param>
        /// <param name="isDefinitionFields">Заполнять дополнительные поля из дефинишена</param>
        /// <returns></returns>
        [HttpGet("structure")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public SiteNode Structure(string dnsName,
            IDictionary<string, string> targeting, string[] fields,
            int? deep, bool isDefinitionFields = false)
        {
            var storage = _abstractItemStorageProvider.Get();

            var startPageFilter = _targetingFiltersFactory.StructureFilter(targeting);

            var startPage = storage.GetStartPage<UniversalPage>(dnsName, startPageFilter);
            if (startPage == null)
                throw new StatusCodeException(System.Net.HttpStatusCode.NotFound);

            var pagesFilters = new OnlyPagesFilter().AddFilter(startPageFilter);
            return new SiteNode(startPage, pagesFilters, deep, fields, isDefinitionFields);
        }
    }
}

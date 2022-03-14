using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QA.WidgetPlatform.Api.Application;
using QA.WidgetPlatform.Api.Models;
using QA.WidgetPlatform.Api.Services.Abstract;

namespace QA.WidgetPlatform.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SiteController : ControllerBase
    {
        private readonly ISiteStructureService _siteStructureService;

        public SiteController(ISiteStructureService siteStructureService)
        {
            _siteStructureService = siteStructureService;
        }

        /// <summary>
        /// Получение структуры страниц сайта
        /// </summary>
        /// <param name="dnsName">Доменное имя сайта</param>
        /// <param name="targeting">Словарь значений таргетирования</param>
        /// <param name="fields">Поля деталей к выдаче. Если пусто, то детали выдаваться не будут</param>
        /// <param name="deep">Глубина страуктуры, где 0 - это корневой элемент</param>
        /// <returns></returns>
        [HttpGet("structure")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public SiteNode Structure([Required] [FromQuery] string dnsName,
            [Bind(Prefix = "t")] [FromQuery] CaseInSensitiveDictionary<string> targeting, [FromQuery] string[] fields,
            int? deep)
            => _siteStructureService.Structure(dnsName, targeting, fields, deep);
    }
}

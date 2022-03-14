using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QA.WidgetPlatform.Api.Models;

namespace QA.WidgetPlatform.Api.Services.Abstract
{
    public interface ISiteStructureService
    {
        /// <summary>
        /// Получение структуры страниц сайта
        /// </summary>
        /// <param name="dnsName">Доменное имя сайта</param>
        /// <param name="targeting">Словарь значений таргетирования. Ключи начинаются с "t"</param>
        /// <param name="fields">Поля деталей к выдаче. Если пусто, то детали выдаваться не будут</param>
        /// <param name="deep">Глубина страуктуры, где 0 - это корневой элемент</param>
        /// <returns></returns>
        SiteNode Structure(string dnsName,
            IDictionary<string, string> targeting, [FromQuery] string[] fields,
            int? deep);
    }
}

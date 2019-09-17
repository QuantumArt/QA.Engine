using QA.DotNetCore.Engine.Abstractions.OnScreen;
using System;

namespace QA.DotNetCore.Engine.OnScreen.Configuration
{
    public class OnScreenSettings
    {
        /// <summary>
        /// Урл компонента Onscreen API (админка onscreen), который работает с тем же QP, что и сайт 
        /// </summary>
        public string AdminSiteBaseUrl { get; set; }
        /// <summary>
        /// Id сайта в QP
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// Режим Stage
        /// </summary>
        public bool IsStage { get; set; }
        /// <summary>
        /// Кастомер-код базы сайта в QP
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// Фичи режима onscreen, которые нужно включить
        /// </summary>
        public OnScreenFeatures AvailableFeatures { get; set; }
        /// <summary>
        /// Имя куки, в которой хранится информация об аутентификации onscreen
        /// </summary>
        public string AuthCookieName { get; set; }
        /// <summary>
        /// Время жизни куки, в которой хранится информация об аутентификации onscreen
        /// </summary>
        public TimeSpan AuthCookieLifetime { get; set; }
        /// <summary>
        /// Имя query-параметра с backend_sid, который создаёт QP для custom action, открытых через фрейм
        /// </summary>
        public string BackendSidQueryKey { get; set; }
        /// <summary>
        /// Под каким именем админка onscreen авторизуется в QP (должно совпадать с настройкой у Onscreen API)
        /// </summary>
        public string ApiApplicationNameInQp { get; set; }
        /// <summary>
        /// Имя куки, в которой хранится переопределенная для режима аб-тестов настройка isStage
        /// </summary>
        public string OverrideAbTestStageModeCookieName { get; set; }
        /// <summary>
        /// Имя query-параметра с текущим Id страницы структуры сайта
        /// </summary>
        public string PageIdQueryParamName { get; set; }
    }
}

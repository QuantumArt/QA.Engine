using System;
using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Caching.Interfaces;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.CacheTags
{
    /// <summary>
    /// Мидлвара для stage-режима, чтобы при каждом запросе отслеживать изменения по кештегам
    /// </summary>
    public class CacheInvalidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _excludePathRegex;

        public CacheInvalidationMiddleware(RequestDelegate next, string excludePathRegex)
        {
            _next = next;
            _excludePathRegex = excludePathRegex;
        }

        public Task Invoke(HttpContext context, ICacheTagWatcher cacheTagWatcher)
        {
            if (!Regex.IsMatch(context.Request.Path, _excludePathRegex))
            {
                cacheTagWatcher.TrackChanges();
            }
            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}

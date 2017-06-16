using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebSite.PagesAndWidgets
{
    public class DemoTargetingFilterAccessor : ITargetingFilterAccessor
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public DemoTargetingFilterAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ITargetingFilter Get()
        {
            var ctx = _httpContextAccessor.HttpContext;

            RegionFilter regionFlt = null;
            var regionStr = ctx.Request.Query["region"].FirstOrDefault();
            if (regionStr == null)
            {
                regionFlt = new RegionFilter(new int[4] { 77507, 77996, 77512, 78043 });
            }
            else
            {
                var regionFilter = new RegionFilter(
                    regionStr.Split(',')
                    .Where(_ => int.TryParse(_, out int tmp))
                    .Select(_ => int.Parse(_))
                    .ToArray()
                );
            }
            
            CultureFilter cultureFlt = null;
            var cultureStr = ctx.Request.Query["culture"].FirstOrDefault();
            cultureFlt = new CultureFilter(cultureStr ?? "ru-ru");

            return regionFlt + cultureFlt;
        }
    }
}

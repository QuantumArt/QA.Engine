using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching
{
    public interface IHeadTokenPossibleValuesAccessor
    {
        string[] GetPossibleValues(string key);
    }

    public class HeadTokenPossibleValuesAccessor : IHeadTokenPossibleValuesAccessor
    {
        private readonly ServiceSetConfigurator<IHeadTokenPossibleValuesProvider> _registeredProviders;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeadTokenPossibleValuesAccessor(ServiceSetConfigurator<IHeadTokenPossibleValuesProvider> registeredProviders,
            IHttpContextAccessor httpContextAccessor)
        {
            _registeredProviders = registeredProviders;
            _httpContextAccessor = httpContextAccessor;
        }

        public string[] GetPossibleValues(string key)
        {
            Dictionary<string, IEnumerable<string>> fullDictionary = null;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.Items.ContainsKey(HttpContextItemKey))
            {
                fullDictionary = (Dictionary<string, IEnumerable<string>>)httpContext.Items[HttpContextItemKey];
            }
            if (fullDictionary == null)
            {
                fullDictionary = GetFullDictionary();
                httpContext.Items[HttpContextItemKey] = fullDictionary;
            }

            if (fullDictionary.ContainsKey(key))
                return fullDictionary[key].ToArray();

            return Array.Empty<string>();
        }

        private const string HttpContextItemKey = "HeadTokenPossibleValues";

        private Dictionary<string, IEnumerable<string>> GetFullDictionary()
        {
            var result = new Dictionary<string, IEnumerable<string>>();

            foreach (var provider in _registeredProviders.GetServices(_httpContextAccessor.HttpContext.RequestServices))
            {
                var dict = provider.GetPossibleValues();
                foreach (var key in dict.Keys)
                {
                    result[key] = dict[key];
                }
            }

            return result;
        }
    }
}

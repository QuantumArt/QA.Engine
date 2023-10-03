using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using System.Collections.Generic;
using System.Linq;

namespace DemoWebSite.PagesAndWidgets
{
    public class DictionariesPossibleValuesProvider : IHeadTokenPossibleValuesProvider
    {
        private readonly IDictionaryProvider _dictionaryProvider;

        public DictionariesPossibleValuesProvider(IDictionaryProvider dictionaryProvider)
        {
            _dictionaryProvider = dictionaryProvider;
        }

        public IDictionary<string, IEnumerable<string>> GetPossibleValues()
        {
            var keys = _dictionaryProvider.GetKeys();

            var dictionary = keys.ToDictionary(
                key => key,
                key => _dictionaryProvider.GetAllDictionaryItems(key).Select(item => item.Alias));

            return dictionary;
        }
    }
}

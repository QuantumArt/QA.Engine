using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Interfaces
{
    public interface IDictionaryProvider
    {
        IEnumerable<DictionaryItem> GetAllDictionaryItems(string key);
        IEnumerable<DictionaryItem> GetParentDictionaryItems(string key, string alias);
        IEnumerable<string> GetKeys();
    }
}

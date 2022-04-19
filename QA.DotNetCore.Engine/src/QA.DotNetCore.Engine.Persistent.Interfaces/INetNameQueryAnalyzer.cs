using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface INetNameQueryAnalyzer
    {
        string PrepareQuery(string netNameQuery, int siteId, bool isStage, bool useUnited = false);
        IEnumerable<string> GetContentNetNames(string netNameQuery, int siteId, bool isStage, bool useUnited = false);
    }
}

namespace QA.DotNetCore.Engine.QpData.Persistent.Interfaces
{
    public interface INetNameQueryAnalyzer
    {
        string PrepareQuery(string netNameQuery, int siteId, bool isStage);
    }
}

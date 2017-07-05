using QA.DotNetCore.Engine.QpData.Persistent.Data;

namespace QA.DotNetCore.Engine.QpData.Persistent.Interfaces
{
    public interface IMetaInfoRepository
    {
        QpSitePersistentData GetSite(int siteId);

        ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName);
    }
}

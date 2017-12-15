using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IMetaInfoRepository
    {
        QpSitePersistentData GetSite(int siteId);

        ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName);

        ContentAttributePersistentData GetContentAttributeByNetName(int contentId, string fieldNetName);

        ContentPersistentData GetContent(string contentNetName, int siteId);
    }
}

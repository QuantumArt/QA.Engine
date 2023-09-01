using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using System.Data;

namespace QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IMetaInfoRepository
    {
        QpSitePersistentData GetSite(int siteId, IDbTransaction transaction = null);

        ContentAttributePersistentData GetContentAttribute(int contentId, string fieldName, IDbTransaction transaction = null);

        ContentAttributePersistentData GetContentAttributeByNetName(int contentId, string fieldNetName, IDbTransaction transaction = null);

        ContentPersistentData GetContent(string contentNetName, int siteId, IDbTransaction transaction = null);

        ContentPersistentData[] GetContents(ICollection<string> contentNetNames, int siteId, IDbTransaction transaction = null);

        ContentPersistentData[] GetContentsById(int[] ids, IDbTransaction transaction = null);
    }
}

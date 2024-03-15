using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.Persistent.Dapper
{
    public static class QpTableNameHelper
    {
        public static string GetTableName(this ContentPersistentData content, bool isStage)
        {
            return GetTableName(content.ContentId, isStage);
        }

        public static string GetTableName(int contentId, bool isStage)
        {
            return isStage ? $"CONTENT_{contentId}_STAGE_NEW" : $"CONTENT_{contentId}_LIVE_NEW";
        }

        public static string GetUnitedTableName(this ContentPersistentData content)
        {
            return GetUnitedTableName(content.ContentId);
        }

        public static string GetUnitedTableName(int contentId)
        {
            return $"CONTENT_{contentId}_UNITED_NEW";
        }

        public static string GetM2MTableName(bool isStage)
        {
            return isStage ? $"item_link_united" : "item_link";
        }
    }
}

namespace QA.DotNetCore.Engine.QpData.Persistent.Data
{
    /// <summary>
    /// Настройки поля в контенте QP. (Таблица CONTENT_ATTRIBUTE)
    /// </summary>
    public class ContentAttributePersistentData
    {
        public int Id { get; set; }

        public int ContentId { get; set; }

        public bool UseSiteLibrary { get; set; }

        public string SubFolder { get; set; }
    }
}

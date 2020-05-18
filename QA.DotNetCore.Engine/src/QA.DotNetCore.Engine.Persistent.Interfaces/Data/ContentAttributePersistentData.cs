namespace  QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    /// <summary>
    /// Настройки поля в контенте QP. (Таблица CONTENT_ATTRIBUTE)
    /// </summary>
    public class ContentAttributePersistentData
    {
        public int Id { get; set; }

        public int ContentId { get; set; }

        public string ContentName { get; set; }

        public string ContentNetName { get; set; }

        public string NetName { get; set; }

        public string ColumnName { get; set; }

        public bool UseSiteLibrary { get; set; }

        public string SubFolder { get; set; }

        public bool UseDefaultFiltration { get; set; }

        public string TypeName { get; set; }

        public int? M2mLinkId { get; set; }

        public string InvariantName { get { return $"field_{Id}"; } }

        public bool IsManyToManyField { get { return TypeName == "Relation" && M2mLinkId.HasValue; } }

        public bool IsFileField { get { return TypeName == "File" || TypeName == "Image" || TypeName == "Dynamic Image"; } }
    }
}

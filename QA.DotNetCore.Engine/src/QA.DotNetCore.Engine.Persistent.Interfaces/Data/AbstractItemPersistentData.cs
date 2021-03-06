namespace  QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class AbstractItemPersistentData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Alias { get; set; }
        public string Title { get; set; }
        public string ZoneName { get; set; }
        public int? ExtensionId { get; set; }
        public int? IndexOrder { get; set; }
        public bool? IsVisible { get; set; }
        public string Discriminator { get; set; }
        public bool IsPage { get; set; }
        public int? VersionOfId { get; set; }
        public bool Published { get; set; }
    }
}

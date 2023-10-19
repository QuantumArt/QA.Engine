namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class DictionaryItemPersistentData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Alias { get; set; }
        public string Title { get; set; }
    }
}

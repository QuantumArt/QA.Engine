namespace QA.DotNetCore.Engine.QpData
{
    public class DictionaryItem
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public DictionaryItem? Parent { get; set;}
        public string Alias { get; set; }
        public string Title { get; set; }
    }
}

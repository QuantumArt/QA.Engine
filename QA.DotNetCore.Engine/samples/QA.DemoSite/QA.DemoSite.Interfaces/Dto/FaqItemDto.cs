namespace QA.DemoSite.Interfaces.Dto
{
    public class FaqItemDto
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int? SortOrder { get; set; }
    }
}

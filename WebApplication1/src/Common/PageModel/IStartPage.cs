namespace Common.PageModel
{
    public interface IStartPage : IAbstractItem
    {
        string[] GetDNSBindings();
    }
}
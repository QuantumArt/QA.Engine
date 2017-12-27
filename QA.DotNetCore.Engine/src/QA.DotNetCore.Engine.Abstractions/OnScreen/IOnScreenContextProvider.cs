namespace QA.DotNetCore.Engine.Abstractions.OnScreen
{
    public interface IOnScreenContextProvider
    {
        void SetContext();
        OnScreenContext GetContext();
    }
}

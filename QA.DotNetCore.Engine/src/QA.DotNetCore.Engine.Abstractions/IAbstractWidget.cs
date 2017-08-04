namespace QA.DotNetCore.Engine.Abstractions
{
    public interface IAbstractWidget : IAbstractItem
    {
        string ZoneName { get; }

        string[] AllowedUrlPatterns { get; }

        string[] DeniedUrlPatterns { get; }
    }
}

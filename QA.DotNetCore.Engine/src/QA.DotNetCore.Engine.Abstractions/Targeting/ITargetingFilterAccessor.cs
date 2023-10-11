namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingFilterAccessor
    {
        ITargetingFilter Get();
        ITargetingFilter Get(TargetingDestination key);
    }
}

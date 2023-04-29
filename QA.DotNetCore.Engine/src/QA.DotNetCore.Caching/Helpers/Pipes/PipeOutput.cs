namespace QA.DotNetCore.Caching.Helpers.Pipes;

public record struct PipeOutput<TResult>(TResult Result, bool IsFinal)
{
}

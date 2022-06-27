namespace QA.DotNetCore.Caching.Helpers.Operations;

public record struct OperationResult<TResult>(TResult Result, bool IsFinal)
{
}

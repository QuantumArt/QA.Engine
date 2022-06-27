using System;

namespace QA.DotNetCore.Caching.Helpers.Operations;

public class OperationContext<TResult>
{
    private readonly OperationResult<TResult>[] _previousResults;

    public OperationContext(OperationResult<TResult>[] previousResults)
    {
        _previousResults = previousResults ?? throw new ArgumentNullException(nameof(previousResults));
    }

    public TResult GetPreviousResult(int index)
    {
        return _previousResults[index].Result;
    }
}

using System;

namespace QA.DotNetCore.Caching.Helpers.Pipes;

public class PipeContext<TResult>
{
    private readonly PipeOutput<TResult>[] _previousResults;

    public PipeContext(PipeOutput<TResult>[] previousResults)
    {
        _previousResults = previousResults ?? throw new ArgumentNullException(nameof(previousResults));
    }

    public TResult GetPreviousResult(int index) => _previousResults[index].Result;
}

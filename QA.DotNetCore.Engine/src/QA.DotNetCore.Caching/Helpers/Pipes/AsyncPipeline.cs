using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace QA.DotNetCore.Caching.Helpers.Pipes;

public class AsyncPipeline<TInput, TResult>
{
    public delegate IAsyncEnumerable<PipeOutput<TResult>> AsyncPipeDelegate(TInput[] keys, PipeContext<TResult> context);

    private readonly List<AsyncPipeDelegate> _pipes;
    private readonly ILogger _logger;

    public AsyncPipeline(ILogger logger)
    {
        _pipes = new List<AsyncPipeDelegate>();
        _logger = logger;
    }

    public AsyncPipeline<TInput, TResult> AddPipe(AsyncPipeDelegate pipe)
    {
        _pipes.Add(pipe);
        return this;
    }

    public async Task<IEnumerable<TResult>> ExecuteAsync(TInput[] inputs)
    {
        var pipelineId = Guid.NewGuid();
        var allResults = new PipeOutput<TResult>[inputs.Length];
        var context = new PipeContext<TResult>(allResults);

        using var pipelineScope = _logger.BeginScope(new Dictionary<string, object> { ["PipelineId"] = pipelineId });

        int stepIndex = 0;
        foreach (var pipe in _pipes)
        {
            _logger.LogTrace(
                "Start pipeline step {PipelineStep}/{PipelineStepsCount} (inputs count: {InputsCount})",
                ++stepIndex,
                _pipes.Count,
                inputs.Length);

            IEnumerable<PipeOutput<TResult>> pipeResult = await pipe(inputs, context).ToArrayAsync();
            allResults.Apply(pipeResult);

            var incompleteResults = allResults.GetIncomplete().ToArray();
            if (incompleteResults.Length <= 0)
            {
                break;
            }

            context = new PipeContext<TResult>(incompleteResults);
            inputs = pipeResult.GetIncomplete(inputs).ToArray();
        }

        return allResults.Select(result => result.Result);
    }
}

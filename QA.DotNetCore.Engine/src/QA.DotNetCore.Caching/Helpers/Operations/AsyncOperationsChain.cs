using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Helpers.Operations;

public class AsyncOperationsChain<TInput, TResult>
{
    public delegate IAsyncEnumerable<OperationResult<TResult>> AsyncOperationDelegate(TInput[] keys, OperationContext<TResult> context);

    private readonly List<AsyncOperationDelegate> _operations;
    private readonly ILogger _logger;

    public AsyncOperationsChain(ILogger logger)
    {
        _operations = new List<AsyncOperationDelegate>();
        _logger = logger;
    }

    public AsyncOperationsChain<TInput, TResult> AddOperation(AsyncOperationDelegate operation)
    {
        _operations.Add(operation);
        return this;
    }

    public async Task<IEnumerable<TResult>> ExecuteAsync(TInput[] inputs)
    {
        var pipelineId = Guid.NewGuid();
        var allResults = new OperationResult<TResult>[inputs.Length];
        var context = new OperationContext<TResult>(allResults);

        using var pipelineScope = _logger.BeginScope(new Dictionary<string, object> { ["PipelineId"] = pipelineId });

        int stepIndex = 0;
        foreach (var operation in _operations)
        {
            _logger.LogTrace(
                "Start pipeline step {PipelineStep}/{PiplineStepsCount} (inputs count: {InputsCount})",
                ++stepIndex,
                _operations.Count,
                pipelineId,
                inputs.Length);

            IEnumerable<OperationResult<TResult>> operationResult = await operation(inputs, context).ToArrayAsync();
            allResults.Apply(operationResult);

            var incompleteResults = allResults.GetIncomplete().ToArray();
            if (incompleteResults.Length <= 0)
            {
                break;
            }

            context = new OperationContext<TResult>(incompleteResults);
            inputs = operationResult.GetIncomplete(inputs).ToArray();
        }

        return allResults.Select(result => result.Result);
    }
}

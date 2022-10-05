using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Caching.Helpers.Operations;

public class OperationsChain<TInput, TResult>
{
    public delegate IEnumerable<OperationResult<TResult>> OperationDelegate(TInput[] inputs, OperationContext<TResult> context);

    private readonly List<OperationDelegate> _operations;
    private readonly ILogger _logger;

    public OperationsChain(ILogger logger)
    {
        _logger = logger;
        _operations = new List<OperationDelegate>();
    }

    public OperationsChain<TInput, TResult> AddOperation(OperationDelegate operation)
    {
        _operations.Add(operation);
        return this;
    }

    public IEnumerable<TResult> Execute(TInput[] inputs)
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
                inputs.Length);

            IEnumerable<OperationResult<TResult>> operationResult = operation(inputs, context).ToArray();
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

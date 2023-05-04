using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace QA.DotNetCore.Caching.Helpers.Pipes;

public class Pipeline<TInput, TResult>
{
    public delegate IEnumerable<PipeOutput<TResult>> PipeDelegate(TInput[] inputs, PipeContext<TResult> context);

    private readonly List<PipeDelegate> _pipes;
    private readonly ILogger _logger;

    public Pipeline(ILogger logger)
    {
        _logger = logger;
        _pipes = new List<PipeDelegate>();
    }

    public Pipeline<TInput, TResult> AddPipe(PipeDelegate pipe)
    {
        _pipes.Add(pipe);
        return this;
    }

    public IEnumerable<TResult> Execute(TInput[] inputs)
    {
        var pipelineId = Guid.NewGuid();
        var allResults = new PipeOutput<TResult>[inputs.Length];
        var context = new PipeContext<TResult>(allResults);

        using var pipelineScope = _logger.BeginScope(new Dictionary<string, object> { ["PipelineId"] = pipelineId });

        int stepIndex = 0;
        foreach (var pipe in _pipes)
        {
            _logger.LogTrace(
                "Start pipeline {Id} step {Step}/{Count} (inputs count: {InputsCount})",
                pipelineId,
                ++stepIndex,
                _pipes.Count,
                inputs.Length);

            IEnumerable<PipeOutput<TResult>> operationResult = pipe(inputs, context).ToArray();
            allResults.Apply(operationResult);

            var incompleteResults = allResults.GetIncomplete().ToArray();
            if (incompleteResults.Length <= 0)
            {
                break;
            }

            context = new PipeContext<TResult>(incompleteResults);
            inputs = operationResult.GetIncomplete(inputs).ToArray();
        }

        return allResults.Select(result => result.Result);
    }
}

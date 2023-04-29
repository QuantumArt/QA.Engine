using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Helpers.Pipes;

public static class PipelineExtensions
{
    public static Pipeline<TInput, TResult> AddPipe<TInput, TResult>(
        this Pipeline<TInput, TResult> chain,
        Func<TInput[], IEnumerable<PipeOutput<TResult>>> pipeHandler)
    {
        if (chain is null)
        {
            throw new ArgumentNullException(nameof(chain));
        }

        return chain.AddPipe((inputs, _) => pipeHandler(inputs));
    }

    public static AsyncPipeline<TInput, TResult> AddPipe<TInput, TResult>(
        this AsyncPipeline<TInput, TResult> chain,
        Func<TInput[], IAsyncEnumerable<PipeOutput<TResult>>> pipeHandler)
    {
        if (chain is null)
        {
            throw new ArgumentNullException(nameof(chain));
        }

        return chain.AddPipe((inputs, _) => pipeHandler(inputs));
    }

    public static Pipeline<TInput, TResult> AddPipe<TInput, TResult>(
        this Pipeline<TInput, TResult> chain,
        Func<TInput[], IEnumerable<TResult>> pipeHandler,
        Predicate<TResult> isFinal = null)
    {
        if (chain is null)
        {
            throw new ArgumentNullException(nameof(chain));
        }

        return chain.AddPipe((inputs) => GetResults(pipeHandler(inputs), isFinal));
    }

    public static AsyncPipeline<TInput, TResult> AddPipe<TInput, TResult>(
        this AsyncPipeline<TInput, TResult> chain,
        Func<TInput[], IAsyncEnumerable<TResult>> pipeHandler,
        Predicate<TResult> isFinal = null)
    {
        if (chain is null)
        {
            throw new ArgumentNullException(nameof(chain));
        }

        return chain.AddPipe((inputs) => GetResultsAsync(pipeHandler(inputs), isFinal));
    }

    private static IEnumerable<PipeOutput<TResult>> GetResults<TResult>(
        IEnumerable<TResult> results,
        Predicate<TResult> isFinal = null)
    {
        isFinal ??= _ => true;

        foreach (var result in results)
        {
            bool isFinalResult = isFinal(result);
            yield return new PipeOutput<TResult>(result, isFinalResult);
        }
    }

    private static async IAsyncEnumerable<PipeOutput<TResult>> GetResultsAsync<TResult>(
        IAsyncEnumerable<TResult> results,
        Predicate<TResult> isFinal = null)
    {
        isFinal ??= _ => true;

        await foreach (var result in results)
        {
            bool isFinalResult = isFinal(result);
            yield return new PipeOutput<TResult>(result, isFinalResult);
        }
    }
}

using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Helpers.Operations;

public static class OperationsChainExtensions
{
    public static OperationsChain<TInput, TResult> AddOperation<TInput, TResult>(
        this OperationsChain<TInput, TResult> chain,
        Func<TInput[], IEnumerable<OperationResult<TResult>>> operationHandler)
    {
        if (chain is null)
        {
            throw new ArgumentNullException(nameof(chain));
        }

        return chain.AddOperation((inputs, _) => operationHandler(inputs));
    }

    public static AsyncOperationsChain<TInput, TResult> AddOperation<TInput, TResult>(
        this AsyncOperationsChain<TInput, TResult> chain,
        Func<TInput[], IAsyncEnumerable<OperationResult<TResult>>> operationHandler)
    {
        if (chain is null)
        {
            throw new ArgumentNullException(nameof(chain));
        }

        return chain.AddOperation((inputs, _) => operationHandler(inputs));
    }

    public static OperationsChain<TInput, TResult> AddOperation<TInput, TResult>(
        this OperationsChain<TInput, TResult> chain,
        Func<TInput[], IEnumerable<TResult>> operationHandler,
        Predicate<TResult> isFinal = null)
    {
        if (chain is null)
        {
            throw new ArgumentNullException(nameof(chain));
        }

        return chain.AddOperation((inputs) => GetResults(operationHandler(inputs), isFinal));
    }

    public static AsyncOperationsChain<TInput, TResult> AddOperation<TInput, TResult>(
        this AsyncOperationsChain<TInput, TResult> chain,
        Func<TInput[], IAsyncEnumerable<TResult>> operationHandler,
        Predicate<TResult> isFinal = null)
    {
        if (chain is null)
        {
            throw new ArgumentNullException(nameof(chain));
        }

        return chain.AddOperation((inputs) => GetResultsAsync(operationHandler(inputs), isFinal));
    }

    private static IEnumerable<OperationResult<TResult>> GetResults<TResult>(
        IEnumerable<TResult> results,
        Predicate<TResult> isFinal = null)
    {
        isFinal ??= (_) => true;

        foreach (var result in results)
        {
            bool isFinalResult = isFinal(result);
            yield return new OperationResult<TResult>(result, isFinalResult);
        }
    }

    private static async IAsyncEnumerable<OperationResult<TResult>> GetResultsAsync<TResult>(
        IAsyncEnumerable<TResult> results,
        Predicate<TResult> isFinal = null)
    {
        isFinal ??= (_) => true;

        await foreach (var result in results)
        {
            bool isFinalResult = isFinal(result);
            yield return new OperationResult<TResult>(result, isFinalResult);
        }
    }
}

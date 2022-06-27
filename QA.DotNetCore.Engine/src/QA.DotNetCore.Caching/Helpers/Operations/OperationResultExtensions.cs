using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Helpers.Operations;

public static class OperationResultExtensions
{
    public static IEnumerable<OperationResult<TResult>> GetIncomplete<TResult>(this IEnumerable<OperationResult<TResult>> results) =>
        results.GetIncomplete(results);

    public static IEnumerable<T> GetIncomplete<TResult, T>(this IEnumerable<OperationResult<TResult>> results, IEnumerable<T> collection)
    {
        using var resultEnumerator = results.GetEnumerator();
        using var collectionEnumerator = collection.GetEnumerator();

        while (collectionEnumerator.MoveNext() && resultEnumerator.MoveNext())
        {
            OperationResult<TResult> result = resultEnumerator.Current;

            if (!result.IsFinal)
            {
                yield return collectionEnumerator.Current;
            }
        }
    }

    public static void Apply<TResult>(
        this OperationResult<TResult>[] allResults,
        IEnumerable<OperationResult<TResult>> currentResults)
    {
        using var currentResultsEnumerator = currentResults.GetEnumerator();

        int currentResultsCount = 0;
        for (int i = 0; i < allResults.Length; i++)
        {
            if (allResults[i].IsFinal)
            {
                continue;
            }

            if (!currentResultsEnumerator.MoveNext())
            {
                throw new InvalidOperationException(
                    $"Invlaid current results count ({currentResultsCount}).");
            }
            currentResultsCount++;

            allResults[i] = currentResultsEnumerator.Current;
        }
    }
}

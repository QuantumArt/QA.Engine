using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Helpers.Pipes;

public static class PipeOutputExtensions
{
    public static IEnumerable<PipeOutput<TResult>> GetIncomplete<TResult>(this IEnumerable<PipeOutput<TResult>> results) =>
        results.GetIncomplete(results);

    public static IEnumerable<T> GetIncomplete<TResult, T>(this IEnumerable<PipeOutput<TResult>> results, IEnumerable<T> collection)
    {
        using var resultEnumerator = results.GetEnumerator();
        using var collectionEnumerator = collection.GetEnumerator();

        while (collectionEnumerator.MoveNext() && resultEnumerator.MoveNext())
        {
            PipeOutput<TResult> result = resultEnumerator.Current;

            if (!result.IsFinal)
            {
                yield return collectionEnumerator.Current;
            }
        }
    }

    public static void Apply<TResult>(
        this PipeOutput<TResult>[] allResults,
        IEnumerable<PipeOutput<TResult>> currentResults)
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
                    $"Invalid current results count ({currentResultsCount}).");
            }
            currentResultsCount++;

            allResults[i] = currentResultsEnumerator.Current;
        }
    }
}

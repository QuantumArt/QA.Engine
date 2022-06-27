using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Helpers.Operations;

public class AsyncOperationsChain<TInput, TResult>
{
    public delegate IAsyncEnumerable<OperationResult<TResult>> AsyncOperationDelegate(TInput[] keys, OperationContext<TResult> context);

    private readonly List<AsyncOperationDelegate> _operations;

    public AsyncOperationsChain()
    {
        _operations = new List<AsyncOperationDelegate>();
    }

    public AsyncOperationsChain<TInput, TResult> AddOperation(AsyncOperationDelegate operation)
    {
        _operations.Add(operation);
        return this;
    }

    public async Task<IEnumerable<TResult>> ExecuteAsync(TInput[] keys)
    {
        var allResults = new OperationResult<TResult>[keys.Length];
        var context = new OperationContext<TResult>(allResults);

        foreach (var operation in _operations)
        {
            IEnumerable<OperationResult<TResult>> operationResults = await operation(keys, context).ToArrayAsync();
            allResults.Apply(operationResults);

            context = new OperationContext<TResult>(allResults.GetIncomplete().ToArray());
            keys = operationResults.GetIncomplete(keys).ToArray();
        }

        return allResults.Select(result => result.Result);
    }
}

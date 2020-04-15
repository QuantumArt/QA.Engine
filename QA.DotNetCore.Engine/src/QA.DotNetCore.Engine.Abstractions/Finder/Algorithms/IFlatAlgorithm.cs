using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions.Finder.Algorithms
{
    public interface IFlatAlgorithm<V>
    {
        IReadOnlyList<TreeNode<IAbstractItem, V>> Run(IAbstractItem root);
    }

}

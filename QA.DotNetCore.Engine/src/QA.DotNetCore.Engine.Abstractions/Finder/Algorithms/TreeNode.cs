using System.Collections.Generic;
using System.Diagnostics;

namespace QA.DotNetCore.Engine.Abstractions.Finder.Algorithms
{
    [DebuggerDisplay("{Item} -> {Data}")]
    public class TreeNode<T, V>
    {
        public T Item { get; private set; }
        public V Data { get; private set; }
        public TreeNode<T, V> Parent { get; private set; }
        public List<TreeNode<T, V>> Children { get; private set; }
        public bool HasChildren { get { return Children != null && Children.Count > 0; } }

        private TreeNode(T node, V data)
        {
            Item = node;
            Data = data;
            Children = new List<TreeNode<T, V>>();
        }

        public TreeNode<T, V> AppendNode(TreeNode<T, V> node)
        {
            node.Parent = this;
            Children.Add(node);
            return this;
        }

        public TreeNode<T, V> PrependNode(TreeNode<T, V> node)
        {
            node.Parent = this;
            Children.Insert(0, node);
            return this;
        }

        public TreeNode<T, V> InsertAt(TreeNode<T, V> node, int index)
        {
            node.Parent = this;
            if (index > Children.Count)
                index = Children.Count - 1;
            if (index < 0)
                index = 0;

            Children.Insert(index, node);
            return this;
        }

        public TreeNode<T, V> AppendRange(IEnumerable<TreeNode<T, V>> nodes)
        {
            foreach (var item in nodes)
            {
                item.Parent = this;
            }

            Children.AddRange(nodes);
            return this;
        }

        public static TreeNode<T, V> CreateNode(T node, V data)
        {
            return new TreeNode<T, V>(node, data);
        }
    }
}

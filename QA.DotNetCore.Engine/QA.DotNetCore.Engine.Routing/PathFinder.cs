using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing
{
    public class PathFinder
    {
        public PathData Find(string path, IAbstractItem root)
        {
            var tokens = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
            {
                return new PathData(root, path);
            }

            IAbstractItem stopItem = root;
            IAbstractItem node = root;
            string remainingPath = path;
            int index = 0;
            foreach (var token in tokens)
            {
                index++;
                node = node.Get(token);
                if (node == null) break;
                stopItem = node;
                remainingPath = $"/{string.Join("/", tokens.Select((x, i) => i < index ? (string)null : x).Where(x => x != null))}";
            }

            return new PathData(stopItem, remainingPath);
        }
    }
}

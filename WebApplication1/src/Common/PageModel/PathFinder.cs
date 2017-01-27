using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public class PathFinder
    {
        public PathData Find(string path, AbstractItem root)
        {
            var tokens = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
            {
                return new PathData(root, path);
            }

            AbstractItem stopItem = root;
            AbstractItem node = root;
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

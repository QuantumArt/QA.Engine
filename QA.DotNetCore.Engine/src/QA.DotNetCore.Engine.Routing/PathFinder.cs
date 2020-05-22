using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using System;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing
{
    public class PathFinder
    {
        public PathFinder(Func<IAbstractItem, bool> stopCondition = null)
        {
            StopCondition = stopCondition;
        }

        public Func<IAbstractItem, bool> StopCondition { get; }

        public PathData Find(string path, IStartPage root, ITargetingFilter targetingFilter, IHeadUrlResolver urlResolver)
        {
            path = urlResolver.SanitizeUrl(path);

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
                if (StopCondition != null && StopCondition(node)) break;
                node = node.Get(token, targetingFilter);
                if (node == null) break;
                index++;
                stopItem = node;
                remainingPath = $"/{string.Join("/", tokens.Select((x, i) => i < index ? (string)null : x).Where(x => x != null))}";
            }

            return new PathData(stopItem, remainingPath);
        }
    }
}

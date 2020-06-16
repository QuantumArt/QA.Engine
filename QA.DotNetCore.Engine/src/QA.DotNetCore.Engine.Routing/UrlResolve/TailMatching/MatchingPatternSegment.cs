using System;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching
{
    internal class MatchingPatternSegment
    {
        public bool Required { get; private set; }
        public string Name { get; private set; }
        public string Default { get; private set; }
        public string ConstValue { get; private set; }
        public string[] VariativeValues { get; private set; }

        public MatchingPatternSegment(string segment)
        {
            if (segment.StartsWith("{") && segment.EndsWith("}"))
            {
                if (segment.Contains("="))
                {
                    Required = false;
                    Name = segment.Substring(1, segment.IndexOf("=") - 1);
                    Default = segment.Substring(segment.IndexOf("=") + 1, segment.Length - segment.IndexOf("=") - 2);
                }
                else if (segment.EndsWith("?}"))
                {
                    Required = false;
                    Name = segment.Substring(1, segment.Length - 3);
                }
                else if (segment.Contains("[") && segment.EndsWith("]}"))
                {
                    int startVariative = segment.IndexOf('[');

                    Required = true;
                    Name = segment.Substring(1, startVariative - 1);
                    VariativeValues = segment.Substring(startVariative + 1, segment.IndexOf(']') - startVariative - 1)
                        .Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    Required = true;
                    Name = segment.Substring(1, segment.Length - 2);
                }
            }
            else
            {
                Required = true;
                ConstValue = segment;
            }
        }
    }
}

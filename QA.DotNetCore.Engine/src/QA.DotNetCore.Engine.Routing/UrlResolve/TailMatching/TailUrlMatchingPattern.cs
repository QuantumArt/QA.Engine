using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching
{
    public class TailUrlMatchingPattern
    {
        public string Pattern { get; set; }
        public Dictionary<string,string> Defaults { get; set; }

        private static MatchingPatternSegment[] ParseSegments(string urlPath)
        {
            if (urlPath == null)
                return null;

            var segments = urlPath
                .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new MatchingPatternSegment(s))
                .ToArray();

            if (!ValidateSegments(segments))
                throw new TailUrlMatchingPatternException("Tail pattern is invalid: required segment comes after optional");

            return segments;
        }

        private static bool ValidateSegments(MatchingPatternSegment[] segments)
        {
            //не должно быть необязательных сегментов перед обязательным
            var lastSegmentIsRequired = true;
            foreach (var segment in segments)
            {
                if (!lastSegmentIsRequired && segment.Required)
                    return false;
                lastSegmentIsRequired = segment.Required;
            }
            return true;
        }

        /// <summary>
        /// Сопоставление хвоста урла шаблону
        /// </summary>
        /// <param name="tailUrl">хвост урла</param>
        /// <returns></returns>
        public TailUrlMatchResult Match(string tailUrl)
        {
            var patternSegments = ParseSegments(Pattern);

            var tailSegments = tailUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (tailSegments.Length > patternSegments.Length)
            {
                //хвост не может содержать больше сегментов, чем шаблон
                return new TailUrlMatchResult { IsMatch = false };
            }

            var matchedRouteValues = new Dictionary<string, string>();
            for (var index = 0; index < patternSegments.Length; index++)
            {
                var patternSegment = patternSegments[index];
                if (tailSegments.Length < index + 1)
                {
                    //если хвост уже закончился, а шаблон еще нет
                    if (patternSegment.Required)
                    {
                        return new TailUrlMatchResult { IsMatch = false };
                    }
                    if (patternSegment.Name != null && patternSegment.Default != null)
                    {
                        matchedRouteValues[patternSegment.Name] = patternSegment.Default;
                    }
                }
                else
                {
                    if (patternSegment.ConstValue != null)
                    {
                        //если в шаблоне на этой позиции задана фиксированная строка, то в хвосте она тоже должна быть
                        if (!patternSegment.ConstValue.Equals(tailSegments[index], StringComparison.InvariantCultureIgnoreCase))
                        {
                            return new TailUrlMatchResult { IsMatch = false };
                        }
                    }
                    else if (patternSegment.Name != null)
                    {
                        matchedRouteValues[patternSegment.Name] = tailSegments[index];
                    }
                }
            }

            //хвост соответствует шаблону
            //добавим Defaults к результирующим RouteValues
            if (Defaults != null)
            {
                foreach (var key in Defaults.Keys)
                {
                    if (!matchedRouteValues.ContainsKey(key))
                    {
                        matchedRouteValues[key] = Defaults[key];
                    }
                }
            }

            return new TailUrlMatchResult { IsMatch = true, Values = matchedRouteValues };
        }

        public class TailUrlMatchingPatternException : Exception
        {
            public TailUrlMatchingPatternException(string message) : base(message)
            { }
        }

        internal class MatchingPatternSegment
        {
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

            public bool Required { get; private set; }
            public string Name { get; private set; }
            public string Default { get; private set; }
            public string ConstValue { get; private set; }
        }
    }
}

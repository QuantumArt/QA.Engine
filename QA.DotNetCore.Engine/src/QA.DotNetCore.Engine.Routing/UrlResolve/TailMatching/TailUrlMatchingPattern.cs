using QA.DotNetCore.Engine.Routing.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching
{
    public class TailUrlMatchingPattern
    {
        public string Pattern { get; set; }
        public Dictionary<string, string> Defaults { get; set; }
        public Dictionary<string, string> Constraints { get; set; }

        private MatchingPatternSegment[] ParseSegments(string urlPath)
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

        private bool ValidateSegments(MatchingPatternSegment[] segments)
        {
            //не должно быть необязательных сегментов перед обязательным
            var lastSegmentIsRequired = true;
            foreach (var segment in segments)
            {
                if (segment.Required && segment.Name != null)
                {
                    //Если есть ограничение, и данный сегмент обязательный,
                    //а регулярка допускает пустое значение, то ошибка будет выброшена
                    //Пока нет деления, на регулярки с "ИЛИ" (^$|\d{5,9})
                    //и регулярки, с "нулевой длинной" ([a-zA-z_\-]*)
                    if (Constraints != null && Constraints.TryGetValue(segment.Name, out string regexPattern) && Regex.IsMatch(string.Empty, regexPattern))
                        throw new IncorrectConstraintOrPatternException(segment.Name, regexPattern);
                }

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
                        if (!patternSegment.ConstValue.Equals(tailSegments[index], StringComparison.OrdinalIgnoreCase))
                        {
                            return new TailUrlMatchResult { IsMatch = false };
                        }
                    }
                    else if (patternSegment.Name != null)
                    {
                        if (Constraints is null || !Constraints.TryGetValue(patternSegment.Name, out string regexPattern))
                        {
                            if (patternSegment.VariativeValues is null || patternSegment.VariativeValues.Length == 0)
                                matchedRouteValues[patternSegment.Name] = tailSegments[index];
                            else
                            {
                                if (CompareSegmentAndVariativeValue(tailSegments[index], patternSegment))
                                    matchedRouteValues[patternSegment.Name] = tailSegments[index];
                                else
                                    return new TailUrlMatchResult
                                    {
                                        IsMatch = false
                                    };
                            }
                        }
                        else
                        {
                            //если в шаблоне указаны ограничения (пока регулярками), то в сегмент хвоста должен подходить под регулярку
                            if (Regex.IsMatch(tailSegments[index], regexPattern))
                                matchedRouteValues[patternSegment.Name] = tailSegments[index];
                            else
                                return new TailUrlMatchResult
                                {
                                    IsMatch = false
                                };
                        }
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

        private static bool CompareSegmentAndVariativeValue(string tailSegment, MatchingPatternSegment patternSegment)
        {
            for (int variativeIndex = 0; variativeIndex < patternSegment.VariativeValues.Length; variativeIndex++)
                if (string.Equals(tailSegment, patternSegment.VariativeValues[variativeIndex], StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }
    }
}

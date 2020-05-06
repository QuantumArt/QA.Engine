using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching
{
    /// <summary>
    /// Результат сопоставления хвоста урла шаблону
    /// </summary>
    public class TailUrlMatchResult
    {
        public bool IsMatch { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
}

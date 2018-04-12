using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.Tests.Fakes
{
    public class FakeTargetingContext : ITargetingContext
    {
        public FakeTargetingContext(Dictionary<string, string> currentValues,
            Dictionary<string, IEnumerable<string>> possibleValues)
        {
            CurrentValues = currentValues;
            PossibleValues = possibleValues;
        }

        public Dictionary<string, string> CurrentValues { get; }
        public Dictionary<string, IEnumerable<string>> PossibleValues { get; }

        public object[] GetPossibleValues(string key)
        {
            if (!PossibleValues.ContainsKey(key))
                return new object[0];
            return PossibleValues[key].Cast<string>().ToArray();
        }

        public string[] GetTargetingKeys()
        {
            return CurrentValues.Keys.ToArray();
        }

        public object GetTargetingValue(string key)
        {
            if (!CurrentValues.ContainsKey(key))
                return null;
            return CurrentValues[key];
        }
    }
}

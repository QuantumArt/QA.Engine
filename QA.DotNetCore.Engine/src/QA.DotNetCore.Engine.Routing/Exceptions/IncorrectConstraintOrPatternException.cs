using System;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Engine.Routing.Exceptions
{
    [Serializable]
    public class IncorrectConstraintOrPatternException : Exception
    {
        public IncorrectConstraintOrPatternException(string segmentName, string regexPattern)
            : base($"Constraint \"{regexPattern}\" allows an empty value of the required segment \"{segmentName}\"") { }
        public IncorrectConstraintOrPatternException(string message, Exception inner) : base(message, inner) { }
        protected IncorrectConstraintOrPatternException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

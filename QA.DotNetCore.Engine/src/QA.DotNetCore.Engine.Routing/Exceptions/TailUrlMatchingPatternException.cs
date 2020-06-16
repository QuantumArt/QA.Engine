using System;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Engine.Routing.Exceptions
{
    [Serializable]
    public class TailUrlMatchingPatternException : Exception
    {
        public TailUrlMatchingPatternException(string message) : base(message) { }
        public TailUrlMatchingPatternException(string message, Exception inner) : base(message, inner) { }
        protected TailUrlMatchingPatternException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

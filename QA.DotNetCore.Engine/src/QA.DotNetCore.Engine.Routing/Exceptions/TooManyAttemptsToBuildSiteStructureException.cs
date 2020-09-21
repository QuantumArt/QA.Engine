using System;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Engine.Routing.Exceptions
{
    [Serializable]
    public class TooManyAttemptsToBuildSiteStructureException : Exception
    {
        public TooManyAttemptsToBuildSiteStructureException() { }
        public TooManyAttemptsToBuildSiteStructureException(string message) : base(message) { }
        public TooManyAttemptsToBuildSiteStructureException(string message, Exception inner) : base(message, inner) { }
        protected TooManyAttemptsToBuildSiteStructureException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

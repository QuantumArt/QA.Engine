using System;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Caching.Exceptions
{
    [Serializable]
    public class DeprecateCacheIsExpiredOrMissingException : Exception
    {
        public DeprecateCacheIsExpiredOrMissingException() { }
        public DeprecateCacheIsExpiredOrMissingException(string message) : base(message) { }
        public DeprecateCacheIsExpiredOrMissingException(string message, Exception inner) : base(message, inner) { }
        protected DeprecateCacheIsExpiredOrMissingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

using System;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Caching.Distributed
{
    [Serializable]
    public class CacheDataSerializationException : ApplicationException
    {
        public object Value { get; }

        public CacheDataSerializationException(string message, object value, Exception inner) : base(message, inner)
        {
            Value = value;
        }

        protected CacheDataSerializationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

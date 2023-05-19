using System;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Caching.Distributed.Internals
{
    [Serializable]
    internal class CacheDataSerializationException<TValue> : ApplicationException
    {
        public TValue Value { get; }

        public CacheDataSerializationException(string message, TValue value, Exception inner) : base(message, inner)
        {
            Value = value;
        }

        protected CacheDataSerializationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

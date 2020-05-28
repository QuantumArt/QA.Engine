using System;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Engine.Routing.Exceptions
{
    [Serializable]
    public class StartPageNotFoundException : Exception
    {        
        public StartPageNotFoundException(string message) : base($"Not found \"start-page\" binding for host: \"{message}\"") { }
        public StartPageNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected StartPageNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

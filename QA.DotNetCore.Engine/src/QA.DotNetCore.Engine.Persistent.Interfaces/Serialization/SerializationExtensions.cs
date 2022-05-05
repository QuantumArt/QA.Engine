using System.Diagnostics;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Serialization
{
    public static class SerializationExtensions
    {
        public static TValue GetValue<TValue>(this SerializationInfo info, string propertyName)
        {
            Debug.Assert(info != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName), $"'{nameof(propertyName)}' cannot be null or whitespace.");

            return (TValue)info.GetValue(propertyName, typeof(TValue));
        }

        public static void AddValue<TValue>(this SerializationInfo info, string propertyName, TValue value)
        {
            Debug.Assert(info != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName), $"'{nameof(propertyName)}' cannot be null or whitespace.");

            info.AddValue(propertyName, value);
        }
    }
}

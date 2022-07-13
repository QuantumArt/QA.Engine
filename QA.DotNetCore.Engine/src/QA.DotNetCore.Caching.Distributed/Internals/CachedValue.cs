using System;

namespace QA.DotNetCore.Caching.Distributed
{
    internal readonly struct CachedValue
    {
        private readonly byte[] _value;

        public static readonly CachedValue Empty = default;

        public KeyState State { get; }
        public byte[] Value => _value ?? Array.Empty<byte>();

        public CachedValue(KeyState state, byte[] value)
        {
            State = state;
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static implicit operator (KeyState, byte[])(CachedValue result)
        {
            return (result.State, result.Value);
        }

        public static implicit operator CachedValue((KeyState State, byte[] Value) result)
        {
            return new CachedValue(result.State, result.Value);
        }

        public override string ToString()
        {
            return $"state={State},length={Value.Length}";
        }
    }
}

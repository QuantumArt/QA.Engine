using System;

namespace QA.DotNetCore.Caching
{
    public class LockCacheKey : CacheKey
    {
        public LockCacheKey(CacheKey key)
            : base(CacheKeyType.Lock, GetLockNumber(key).ToString(), key.Instance)
        {
        }

        private static uint GetLockNumber(CacheKey key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            const int maxLocksNumber = 65536;
            return (uint)key.GetHashCode() % maxLocksNumber;
        }
    }
}

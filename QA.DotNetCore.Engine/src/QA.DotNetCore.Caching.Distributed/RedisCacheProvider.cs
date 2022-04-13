using Newtonsoft.Json;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Distributed
{
    /// <summary>
    /// Adapter for IDistributedTaggedCache.
    /// </summary>
    public class RedisCacheProvider : ICacheProvider, IDisposable
    {
        private bool disposedValue;
        private readonly IDistributedTaggedCache _cache;
        private static readonly JsonSerializer s_serializer = JsonSerializer.CreateDefault();

        public RedisCacheProvider(IDistributedTaggedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private static byte[] ConvertToData<T>(T value)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                s_serializer.Serialize(jsonWriter, value);
                jsonWriter.Flush();
                writer.Flush();
                return stream.GetBuffer();
            }
        }

        private static T ConvertFromData<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return s_serializer.Deserialize<T>(jsonReader);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    _cache.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public void Set(string key, object data, int cacheTimeInSeconds) =>
            _cache.Set(key, Enumerable.Empty<string>(), TimeSpan.FromSeconds(cacheTimeInSeconds), ConvertToData(data));

        public void Set(string key, object data, TimeSpan expiration) =>
            _cache.Set(key, Enumerable.Empty<string>(), expiration, ConvertToData(data));

        public bool IsSet(string key) =>
            _cache.IsExists(key);

        public bool TryGetValue(string key, out object result)
        {
            byte[] cachedData = _cache.Get(key);

            result = cachedData != null
                ? ConvertFromData<object>(cachedData)
                : null;
            return result != null;
        }

        public void Invalidate(string key) =>
            _cache.Invalidate(key);

        public void Add(object value, string key, string[] tags, TimeSpan expiration) =>
            _cache.Set(key, tags, expiration, ConvertToData(value));

        public void InvalidateByTag(string tag) =>
            _cache.InvalidateTag(tag);

        public void InvalidateByTags(params string[] tags)
        {
            foreach (var tag in tags)
                _cache.InvalidateTag(tag);
        }

        public T GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getValue, TimeSpan waitForCalculateTimeout = default) =>
            GetOrAdd(cacheKey, Array.Empty<string>(), expiration, getValue, waitForCalculateTimeout);

        public T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getValue, TimeSpan waitForCalculateTimeout = default)
        {
            byte[] getData() => ConvertToData(getValue());

            try
            {
                using (var timeoutSource = CreateTimeoutSource(waitForCalculateTimeout))
                {
                    byte[] data = _cache.GetOrAdd(cacheKey, tags, expiration, getData);
                    return ConvertFromData<T>(data);
                }
            }
            catch (OperationCanceledException)
            {
                return default;
            }
        }

        public Task<T> GetOrAddAsync<T>(string cacheKey, TimeSpan expiration, Func<Task<T>> getValue, TimeSpan waitForCalculateTimeout = default) =>
            GetOrAddAsync(cacheKey, Array.Empty<string>(), expiration, getValue, waitForCalculateTimeout);

        public async Task<T> GetOrAddAsync<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<Task<T>> getValue, TimeSpan waitForCalculateTimeout = default)
        {
            async Task<byte[]> getData() => ConvertToData(await getValue());

            try
            {
                using (var timeoutSource = CreateTimeoutSource(waitForCalculateTimeout))
                {
                    byte[] data = await _cache.GetOrAddAsync(cacheKey, tags, expiration, getData);
                    return ConvertFromData<T>(data);
                }
            }
            catch (OperationCanceledException)
            {
                return default;
            }
        }

        private CancellationTokenSource CreateTimeoutSource(TimeSpan timeout = default)
        {
            return timeout == default
                ? new CancellationTokenSource()
                : new CancellationTokenSource(timeout);
        }
    }
}

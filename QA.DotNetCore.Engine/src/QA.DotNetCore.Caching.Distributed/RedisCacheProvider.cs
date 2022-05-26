using Newtonsoft.Json;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Distributed
{
    /// <summary>
    /// Adapter from IDistributedTaggedCache to ICacheProvider.
    /// </summary>
    public class RedisCacheProvider : ICacheProvider, ICacheInvalidator, IDisposable
    {
        private static readonly JsonSerializer s_serializer = JsonSerializer.CreateDefault();

        private bool _disposedValue;
        private readonly IDistributedTaggedCache _cache;

        public RedisCacheProvider(IDistributedTaggedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private static MemoryStream SerializeData<T>(T value)
        {
            var stream = new MemoryStream();

            using (var writer = new StreamWriter(stream, Encoding.UTF8, -1, leaveOpen: true))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                try
                {
                    s_serializer.Serialize(jsonWriter, value);
                }
                catch (IOException)
                {
                    // TODO: Log it.
                    return new MemoryStream();
                }
                jsonWriter.Flush();
                return stream;
            }
        }

        private static T DeserializeData<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data, false))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return s_serializer.Deserialize<T>(jsonReader);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _cache.Dispose();
                }

                _disposedValue = true;
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
            _cache.Set(key, Enumerable.Empty<string>(), TimeSpan.FromSeconds(cacheTimeInSeconds), SerializeData(data));

        public void Set(string key, object data, TimeSpan expiration) =>
            _cache.Set(key, Enumerable.Empty<string>(), expiration, SerializeData(data));

        public bool IsSet(string key) =>
            _cache.IsExists(key);

        public bool TryGetValue(string key, out object result)
        {
            byte[] cachedData = _cache.Get(key);

            result = cachedData != null
                ? DeserializeData<object>(cachedData)
                : null;
            return result != null;
        }

        public void Add(object value, string key, string[] tags, TimeSpan expiration) =>
            _cache.Set(key, tags, expiration, SerializeData(value));

        public T GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getValue, TimeSpan waitForCalculateTimeout = default) =>
            GetOrAdd(cacheKey, Array.Empty<string>(), expiration, getValue, waitForCalculateTimeout);

        public T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getValue, TimeSpan waitForCalculateTimeout = default)
        {
            MemoryStream getData() => SerializeData(getValue());

            try
            {
                using (var timeoutSource = CreateTimeoutSource(waitForCalculateTimeout))
                {
                    byte[] data = _cache.GetOrAdd(cacheKey, tags, expiration, getData, timeoutSource.Token);
                    return DeserializeData<T>(data);
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
            async Task<MemoryStream> getData() => SerializeData(await getValue());

            try
            {
                using (var timeoutSource = CreateTimeoutSource(waitForCalculateTimeout))
                {
                    byte[] data = await _cache.GetOrAddAsync(cacheKey, tags, expiration, getData);
                    return DeserializeData<T>(data);
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

        public void Invalidate(string key) =>
            _cache.Invalidate(key);

        public void InvalidateByTag(string tag) =>
            _cache.InvalidateTag(tag);

        public void InvalidateByTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _cache.InvalidateTag(tag);
            }
        }
    }
}

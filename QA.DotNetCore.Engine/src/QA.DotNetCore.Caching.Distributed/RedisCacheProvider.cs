using ICSharpCode.SharpZipLib.GZip;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Distributed
{
    /// <summary>
    /// Adapter from IDistributedTaggedCache to ICacheProvider.
    /// </summary>
    public class RedisCacheProvider : ICacheProvider, ICacheInvalidator, IDistributedCacheProvider, IDisposable
    {
        private static readonly JsonSerializer _serializer = JsonSerializer.CreateDefault(
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            });
        private static readonly TimeSpan _defaultLockEnterTimeout = TimeSpan.FromSeconds(5);

        private bool _disposedValue;
        private readonly IDistributedTaggedCache _cache;
        private readonly ILogger<RedisCacheProvider> _logger;

        public RedisCacheProvider(IDistributedTaggedCache cache, ILogger<RedisCacheProvider> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger;
        }

        public IEnumerable<TResult> Get<TResult>(IEnumerable<string> keys)
        {
            var dataValues = _cache.Get(keys);

            foreach ((string key, byte[] data) in keys.Zip(dataValues))
            {
                yield return TryDeserializeData(key, data, out TResult value)
                    ? value
                    : default;
            }
        }

        public IEnumerable<bool> IsSet(IEnumerable<string> keys) =>
            _cache.Exist(keys);

        public bool TryGetValue<TResult>(string key, out TResult result)
        {
            try
            {
                byte[] cachedData = _cache.Get(key);

                return TryDeserializeData(key, cachedData, out result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unable to get cached data associated with the key {CacheKey}.",
                    key);

                result = default;
                return false;
            }
        }

        public void Add(object value, string key, string[] tags, TimeSpan expiration)
        {
            try
            {
                var dataStream = SerializeData(value);
                _cache.Set(key, tags, expiration, dataStream);
            }
            catch (CacheDataSerializationException<object> ex)
            {
                _logger.LogError(ex, "Unable to cache value with the key {CacheKey}.", key);
                Debug.Fail("Unable to cache value with the key " + key);
            }
        }

        public T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getValue, TimeSpan lockEnterTimeout = default)
        {
            MemoryStream getData() => SerializeData(getValue());

            try
            {
                FixupZeroLockEnterTimeout(ref lockEnterTimeout);

                byte[] data = _cache.GetOrAdd(cacheKey, tags, expiration, getData, lockEnterTimeout);
                return DeserializeData<T>(data);
            }
            catch (CacheDataSerializationException<T> ex)
            {
                _logger.LogError(ex, "Unable to cache value with the key {CacheKey}", cacheKey);
                Debug.Fail("Unable to cache value with the key " + cacheKey);

                return ex.Value;
            }
        }

        public TResult[] GetOrAdd<TId, TResult>(
            CacheInfo<TId>[] cacheInfos,
            DataValuesFactoryDelegate<TId, TResult> dataValuesFactory,
            TimeSpan lockEnterTimeout = default)
        {
            IEnumerable<MemoryStream> dataStreamsFactory(IEnumerable<CacheInfo<TId>> missingCacheInfos) =>
                dataValuesFactory(missingCacheInfos).Select(SerializeData);

            FixupZeroLockEnterTimeout(ref lockEnterTimeout);

            var dataResults = _cache.GetOrAdd(cacheInfos, dataStreamsFactory, lockEnterTimeout);
            return dataResults.Select(DeserializeData<TResult>).ToArray();
        }

        public async Task<T> GetOrAddAsync<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<Task<T>> getValue, TimeSpan lockEnterTimeout = default)
        {
            async Task<MemoryStream> getData() => SerializeData(await getValue());

            try
            {
                FixupZeroLockEnterTimeout(ref lockEnterTimeout);

                byte[] data = await _cache.GetOrAddAsync(cacheKey, tags, expiration, getData, lockEnterTimeout);
                return DeserializeData<T>(data);
            }
            catch (CacheDataSerializationException<T> ex)
            {
                _logger.LogError(ex, "Unable to cache value with the key {CacheKey}", cacheKey);
                Debug.Fail("Unable to cache value with the key " + cacheKey);

                return ex.Value;
            }
        }

        public void Invalidate(string key) =>
            _cache.Invalidate(key);

        public void InvalidateByTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _cache.InvalidateTag(tag);
            }
        }

        #region Disposable

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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

        #endregion

        private void FixupZeroLockEnterTimeout(ref TimeSpan lockEnterTimeout)
        {
            if (lockEnterTimeout == default)
            {
                lockEnterTimeout = _defaultLockEnterTimeout;
            }
        }

        private bool TryDeserializeData<TResult>(string key, byte[] data, out TResult result)
        {
            try
            {
                if (data != null)
                {
                    result = DeserializeData<TResult>(data);
                    return result != null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unable to deserialize cached data associated with the key {CacheKey} to type {Type}. " +
                    "Try to erase inconsistent data from cache.",
                    key,
                    typeof(TResult));
            }

            result = default;
            return false;
        }

        private static MemoryStream SerializeData<T>(T data)
        {
            try
            {
                var stream = new MemoryStream();
                var compressedStream = new GZipOutputStream(stream);
                using var writer = new StreamWriter(compressedStream, Encoding.UTF8, bufferSize: -1, leaveOpen: true);
                using var jsonWriter = new JsonTextWriter(writer);

                _serializer.Serialize(jsonWriter, data);
                jsonWriter.Flush();
                compressedStream.Finish();

                return stream;
            }
            catch (Exception ex)
            {
                throw new CacheDataSerializationException<T>("Unable to serialize value to cache.", data, ex);
            }
        }

        private static T DeserializeData<T>(byte[] data)
        {
            using var stream = new MemoryStream(data, false);
            using var decompressedStream = new GZipInputStream(stream);
            using var reader = new StreamReader(decompressedStream, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(reader);

            return _serializer.Deserialize<T>(jsonReader);
        }
    }
}

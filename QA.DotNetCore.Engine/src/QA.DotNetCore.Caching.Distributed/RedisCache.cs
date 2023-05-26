using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using QA.DotNetCore.Caching.Distributed.Internals;

namespace QA.DotNetCore.Caching.Distributed
{
    /// <summary>
    /// Distributed cache using Redis.
    /// <para>Uses <c>StackExchange.Redis</c> as the Redis client.</para>
    /// </summary>
    public partial class RedisCache : IExternalCache
    {
        /// <summary>
        /// Sets tag new expiry only if its exceeds current expiry.
        /// <list type="table">
        ///     <item>KEYS[1] - Tag key</item>
        ///     <item>ARGV[1] - New tag expiry in milliseconds</item>
        /// </list>
        /// </summary>
        /// 
        /// <remarks>
        /// Tags can be linked to multiple keys. That is why
        /// a key should be allowed only to increase tag expiry
        /// (if it has been specified previously by another key).
        /// Otherwise it can lead to a situation when a tag is expired
        /// before some of its keys.
        /// </remarks>
        private const string ExtendKeyExpiryScript = @"
            local key = KEYS[1]
            local key_expiry = tonumber(redis.call('PTTL', key))
            local new_expiry = tonumber(ARGV[1])

            if new_expiry > key_expiry then
              redis.call('PEXPIRE', key, new_expiry)
            end
            ";

        /// <summary>
        /// Invalidates tag and associated keys.
        /// <list type="table">
        ///     <item>KEYS[1] - Tag key to invalidate</item>
        /// </list>
        /// </summary>
        private const string InvalidateTagScript = @"
            local tag = KEYS[1]

            local keys = redis.call('SMEMBERS', tag)
            redis.call('DEL', tag)

            for i=1,#keys do
              redis.call('DEL', keys[i])
            end
            ";

        private static readonly Version _leastSupportedServerVersion = new(4, 0, 0);

        private readonly RedisCacheSettings _options;
        private readonly ILogger<RedisCache> _logger;
        private readonly SemaphoreSlim _connectionLock = new(initialCount: 1, maxCount: 1);

        private volatile IConnectionMultiplexer _connection;
        private IDatabase _cache;
        private bool _disposed;

        private static readonly JsonSerializer _serializer = JsonSerializer.CreateDefault(
            new JsonSerializerSettings
            {
                ContractResolver = new WritablePropertiesOnlyResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            });

        /// <summary>
        /// Initializes a new instance of <see cref="RedisCache"/>.
        /// </summary>
        /// <param name="optionsAccessor">The configuration options.</param>
        public RedisCache(
            IOptions<RedisCacheSettings> optionsAccessor,
            ILogger<RedisCache> logger)
        {
            if (optionsAccessor is null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            _options = optionsAccessor.Value;
            _logger = logger;
        }


        /// <inheritdoc/>
        public bool TryAdd(object value, string key, string deprecatedKey, string[] tags, TimeSpan expiration, TimeSpan deprecatedExpiration)
        {
            var result = true;
            try
            {
                var dataStream = SerializeData(value);
                Set(key, tags, expiration, dataStream, deprecatedKey, deprecatedExpiration);
            }
            catch (CacheDataSerializationException<object> ex)
            {
                _logger.LogInformation(ex, "Unable to cache value with the key {CacheKey}.", key);
                result = false;
            }

            return result;
        }
        
        public void InvalidateByTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                InvalidateTag(tag);
            }
        }


        public IEnumerable<bool> Exist(IEnumerable<string> keys, CancellationToken token = default)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (!keys.Any())
            {
                return Enumerable.Empty<bool>();
            }

            RedisKey[] dataKeys = keys
                .Select(key => new RedisKey(key))
                .ToArray();

            Stopwatch watch = Stopwatch.StartNew();

            Connect(token);
            var existFlags = dataKeys
                .Select(key => _cache.KeyExists(key))
                .ToList();

            _logger.LogTrace(
                "Keys {Keys} exist {ExistFlags} (Elapsed: {Elapsed})",
                keys,
                existFlags,
                watch.ElapsedMilliseconds);

            return existFlags;
        }
        public IEnumerable<TResult> Get<TResult>(IEnumerable<string> keys)
        {
            var dataValues = Get(keys);

            foreach ((string key, byte[] data) in keys.Zip(dataValues))
            {
                yield return TryDeserializeData(key, data, out TResult value)
                    ? value
                    : default;
            }
        }

        public IEnumerable<byte[]> Get(IEnumerable<string> keys, CancellationToken token = default)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (!keys.Any())
            {
                return Enumerable.Empty<byte[]>();
            }

            var redisKeys = keys
                .Select(k => new RedisKey(k))
                .ToArray();

            Stopwatch watch = Stopwatch.StartNew();

            Connect(token);

            var cachedData = GetWithStates(redisKeys)
                .Select(result => result.State == KeyState.Exist ? result.Value : null)
                .ToList();

            _logger.LogTrace("Obtained cached data (Elapsed: {Elapsed})", watch.ElapsedMilliseconds);

            return cachedData;
        }

        public void Invalidate(string key, CancellationToken token = default)
        {
            Connect(token);
            var dataKey = new RedisKey(key);
            _ = _cache.KeyDelete(dataKey);
        }

        public void InvalidateTag(string tag, CancellationToken token = default)
        {
            Connect(token);
            var tagKey = new RedisKey(tag);
            _cache.ScriptEvaluate(InvalidateTagScript, new[] {tagKey});
       }

        public void Set(
            string key,
            IEnumerable<string> tags,
            TimeSpan expiry,
            MemoryStream dataStream,
            string deprecatedKey = null,
            TimeSpan deprecatedExpiry = default,
            CancellationToken token = default)
        {
            if (tags is null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            var dataKey = new RedisKey(key);
            var deprecatedDataKey = new RedisKey(deprecatedKey);
            var tagKeys = tags.Select(k => new RedisKey(k)).ToArray();

            Connect(token);

            _ = TrySet(dataKey, deprecatedDataKey, tagKeys, expiry, deprecatedExpiry, RedisValue.CreateFrom(dataStream));
        }

        private bool TrySet(
            RedisKey key,
            RedisKey deprecatedKey,
            IEnumerable<RedisKey> tags,
            TimeSpan expiry,
            TimeSpan deprecatedExpiry,
            RedisValue data,
            IEnumerable<Condition> conditions = null)
        {
            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                ITransaction transaction = CreateSetCacheTransaction(key, deprecatedKey, tags, expiry, deprecatedExpiry, data, conditions, out var transactionOperations);
                bool isExecuted = transaction.Execute();

                var exceptions = transactionOperations
                    .Where(operation => operation.IsFaulted)
                    .Select(operation => operation.Exception);

                if (exceptions.Any())
                {
                    throw new AggregateException(exceptions);
                }

                _logger.LogInformation(
                    "Set cache operation is finished " +
                    "(isSet: {SetTransactionStatus}, expiry: {Expiration}, key: {CacheKey}, elapsed: {Elapsed}).",
                    isExecuted,
                    expiry,
                    key,
                    watch.ElapsedMilliseconds);

                return isExecuted;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unable to set cache with the key {CacheKey} (Elapsed: {Elapsed})",
                    key,
                    watch.ElapsedMilliseconds);

                return false;
            }
        }

        private ITransaction CreateSetCacheTransaction(
            RedisKey key,
            RedisKey deprecatedKey,
            IEnumerable<RedisKey> tags,
            TimeSpan expiry,
            TimeSpan deprecatedExpiry,
            RedisValue data,
            IEnumerable<Condition> conditions,
            out IEnumerable<Task> transactionOperations)
        {
            ITransaction transaction = _cache.CreateTransaction();

            foreach (var condition in conditions ?? Enumerable.Empty<Condition>())
            {
                _ = transaction.AddCondition(condition);
            }

            var operations = new List<Task>
            {
                transaction.StringSetAsync(key, data, expiry)
            };

            if (!string.IsNullOrEmpty(deprecatedKey) && deprecatedExpiry != default)
            {
                operations.Add(transaction.KeyCopyAsync(key, deprecatedKey, replace: true));
                operations.Add(transaction.KeyExpireAsync(deprecatedKey, deprecatedExpiry));
            }

            RedisValue tagExpiry = (long)GetTagExpiry(expiry).TotalMilliseconds;
            RedisValue keyValue = new RedisValue(key.ToString());

            foreach (var tag in tags)
            {
                operations.Add(transaction.SetAddAsync(tag, keyValue));
                operations.Add(transaction.ScriptEvaluateAsync(
                    ExtendKeyExpiryScript,
                    new[] { tag },
                    new[] { tagExpiry }));
            }

            transactionOperations = operations;
            return transaction;
        }

        private IEnumerable<CachedValue> GetWithStates(RedisKey[] redisKeys)
        {
            try
            {
                var cachedValues = _cache
                    .StringGet(redisKeys)
                    .Select(value => 
                        value.HasValue ? 
                        new CachedValue(KeyState.Exist, (byte[]) value) : 
                        CachedValue.Empty);
                
                _logger.LogTrace("Keys ({CacheKeys}) have values: {CacheValues}", redisKeys, cachedValues);
                return cachedValues;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get multiple caches ({CacheKeys}).", redisKeys);
                return Enumerable.Repeat(CachedValue.Empty, redisKeys.Length);
            }
        }

        private TimeSpan GetTagExpiry(TimeSpan keyExpiry) =>
            keyExpiry + _options.TagExpirationOffset;

        private void Connect(CancellationToken token)
        {
            CheckDisposed();
            token.ThrowIfCancellationRequested();

            if (_cache != null)
            {
                return;
            }

            _connectionLock.Wait(token);
            try
            {
                if (_cache != null)
                {
                    return;
                }

                _connection = ConnectionMultiplexer.Connect(_options.Configuration);
                ValidateServerFeatures();
                _cache = _connection.GetDatabase();
            }
            finally
            {
                _ = _connectionLock.Release();
            }
        }

        private void ValidateServerFeatures()
        {
            _ = _connection ?? throw new InvalidOperationException($"{nameof(_connection)} cannot be null.");

            foreach (var endPoint in _connection.GetEndPoints())
            {
                if (_connection.GetServer(endPoint).Version < _leastSupportedServerVersion)
                {
                    throw new InvalidOperationException(
                        $"Redis version of endpoint {endPoint} ({_connection.GetServer(endPoint).Version}) " +
                        $"is not supported. Should be >={_leastSupportedServerVersion}.");
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _connection?.Close();
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
        
        private MemoryStream SerializeData<T>(T data)
        {
            try
            {
                var stream = new MemoryStream();
                Stream targetStream = (_options.UseCompression) ? new GZipOutputStream(stream) : stream;
                using var writer = new StreamWriter(targetStream, Encoding.UTF8, bufferSize: -1, leaveOpen: true);
                using var jsonWriter = new JsonTextWriter(writer);

                _serializer.Serialize(jsonWriter, data, typeof(T));
                jsonWriter.Flush();
                targetStream.Flush();

                return stream;
            }
            catch (Exception ex)
            {
                throw new CacheDataSerializationException<T>("Unable to serialize value to cache.", data, ex);
            }
        }

        private T DeserializeData<T>(byte[] data)
        {
            using var stream = new MemoryStream(data, false);
            Stream sourceStream = (_options.UseCompression) ? new GZipInputStream(stream) : stream;
            using var reader = new StreamReader(sourceStream, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(reader);

            return _serializer.Deserialize<T>(jsonReader);
        }
        
        private bool TryDeserializeData<TResult>(string key, byte[] data, out TResult result)
        {
            try
            {
                if (data != null)
                {
                    if (data.GetType() == typeof(TResult))
                    {
                        result = (TResult)(object)data;
                    }
                    else
                    {
                        result = DeserializeData<TResult>(data);                   
                    }
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

    }
}

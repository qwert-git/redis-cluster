using System.Diagnostics;
using Newtonsoft.Json;
using StackExchange.Redis;
using RedisClient;
using System;
using System.Threading;

internal class StoreWithProbabilisticCache
{
    private readonly IDatabase _cache;
    private readonly Random _random;
    private readonly int _beta;
    private readonly int _expireCacheTimeInSeconds;

    public StoreWithProbabilisticCache(int beta, int expirationTime)
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(new ConfigurationOptions { EndPoints = { "localhost:6379" } });
        _cache = redis.GetDatabase();

        _random = new Random();
        _beta = beta;
        _expireCacheTimeInSeconds = expirationTime;
    }

    public string Retrieve(int id)
    {
        var cacheKey = GetCacheKey(id);
        var item = GetCacheValueInternal<string>(cacheKey);
        if (item is null || IsCacheExpireSoon(item))
        {
            Console.WriteLine($"Cache with key '{cacheKey}' updating...");

            var sw = Stopwatch.StartNew();

            var value = GetValueFromSource(id);

            item = new CacheItem<string>(value, sw.ElapsedMilliseconds, DateTimeOffset.UtcNow.AddSeconds(_expireCacheTimeInSeconds).ToUnixTimeMilliseconds()); ;

            _cache.StringSet(new RedisKey(cacheKey), new RedisValue(JsonConvert.SerializeObject(item)), TimeSpan.FromSeconds(_expireCacheTimeInSeconds));
        }

        return item.Value;
    }

    public void Add(int id, string value)
    {
        var cacheKey = GetCacheKey(id);

        var sw = Stopwatch.StartNew();

        var item = new CacheItem<string>(value, sw.ElapsedMilliseconds, DateTimeOffset.UtcNow.AddSeconds(_expireCacheTimeInSeconds).ToUnixTimeMilliseconds()); ;

        _cache.StringSet(new RedisKey(cacheKey), new RedisValue(JsonConvert.SerializeObject(item)), TimeSpan.FromSeconds(_expireCacheTimeInSeconds));
    }

    private string GetCacheKey(int id) => $"ProbabilisticCacheTest:ExpireCache:{id}";

    private string GetValueFromSource(int id)
    {
        // Retrieve value from the database...
        Thread.Sleep(1000);

        return $"valueFromTheDatabaseWithId{id}";
    }

    private CacheItem<T>? GetCacheValueInternal<T>(string key)
    {
        var cache = _cache.StringGet(new RedisKey(key));
        return cache.HasValue ? JsonConvert.DeserializeObject<CacheItem<T>>(cache.ToString()) : null;
    }

    private bool IsCacheExpireSoon(CacheItem<string> item)
    {
        var probabilityToUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - item.Delta * _beta * Math.Log(_random.NextDouble());

        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {item.Value} cache, probability diff = {Math.Abs(probabilityToUpdate - item.ExpirationTime)}");

        return probabilityToUpdate >= item.ExpirationTime;
    }
}
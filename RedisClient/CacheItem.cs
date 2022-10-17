namespace RedisClient;

internal class CacheItem<T>
{
    public T Value { get; set; }

    public long Delta { get; set; }

    public long ExpirationTime { get; set; }

    public CacheItem () {}

    public CacheItem(T value, long delta, long expirationTime) 
    {
        Value = value;
        Delta = delta;
        ExpirationTime = expirationTime;
    }
}
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ApiCachingApp.Services;

public class CacheService : ICacheService
{
    private IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetData<T>(string key)
    {
        var jsonData = await _cache.GetStringAsync(key);

        if (jsonData is null)
            return default;

        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public async Task SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = expirationTime
        };

        var jsonData = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(key, jsonData, options);
    }

    public async Task RemoveData(string key)
    {
        await _cache.RemoveAsync(key);
    }
}
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MusicService.Application.Interfaces.Caching;

namespace MusicService.Infrastructure.Caching;

public class RedisService(IDistributedCache cache) : ICachingService
{
    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan ttl) where T : class
    {
        try
        {
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl };
            await cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
            return true;
        }
        catch
        {
            return false;   
        }
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var stringValue = await cache.GetStringAsync(key);
            return string.IsNullOrEmpty(stringValue) ? null : JsonSerializer.Deserialize<T>(stringValue);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        try
        {
            await cache.RemoveAsync(key);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
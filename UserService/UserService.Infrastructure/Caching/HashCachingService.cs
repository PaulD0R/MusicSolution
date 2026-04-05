using System.Text.Json;
using StackExchange.Redis;
using UserService.Application.Interfaces.Cachings;

namespace UserService.Infrastructure.Caching;

public class HashCachingService(IDatabase dataBase) :  IHashCachingService
{
    private const int CacheTime = 15;
    
    public async Task SetAsync<T>(string key, T value)
    {
        var dict = value!.GetType().GetProperties()
            .ToDictionary(
                p => p.Name,
                p => (RedisValue)JsonSerializer.Serialize(p.GetValue(value) ?? "")
            );

        await dataBase.HashSetAsync(key, dict.Select(x =>
            new HashEntry(x.Key, x.Value)).ToArray());
        await dataBase.KeyExpireAsync(key, TimeSpan.FromMinutes(CacheTime));
    }

    public async Task SetFieldAsync<T>(string key, string field, T? value)
    {
        var serialized = JsonSerializer.Serialize(value);
        await dataBase.HashSetAsync(key, field, serialized);
    }

    public async Task<Dictionary<string, string?>?> GetFieldAsync(string key, params string[] fields)
    {
        var redisFields = fields.Select(f => (RedisValue)f).ToArray();
        var values = await dataBase.HashGetAsync(key, redisFields);

        if (values.All(v => !v.HasValue))
            return null;

        var result = new Dictionary<string, string?>();
        for (var i = 0; i < fields.Length; i++)
        {
            result[fields[i]] = values[i].HasValue ? values[i].ToString() : null;
        }

        return result;
    }

    public async Task<Dictionary<string, string>?> GetAsync(string key)
    {
        var entries = await dataBase.HashGetAllAsync(key);

        return entries.Length == 0 ? null 
            : entries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
    }

    public async Task<T?> GetAsync<T>(string key) where T : class, new()
    {
        var entries = await dataBase.HashGetAllAsync(key);
        if (entries.Length == 0) return null;

        var obj = new T();
        var type = typeof(T);
        foreach (var entry in entries)
        {
            var property = type.GetProperty(entry.Name.ToString());
            if (property == null || !property.CanWrite) continue;

            var deserialized = JsonSerializer.Deserialize(entry.Value!.ToString(), property.PropertyType);
            property.SetValue(obj, deserialized);
        }

        return obj;
    }

    public async Task<bool> RemoveFieldAsync(string key, string field)
    {
        return await dataBase.HashDeleteAsync(key, field);
    }
    
    public async Task<bool> RemoveAsync(string key)
    {
        return await dataBase.KeyDeleteAsync(key);
    }
    
    public async Task<bool> KeyExistsAsync(string key)
    {
        return await dataBase.KeyExistsAsync(key);
    }
}
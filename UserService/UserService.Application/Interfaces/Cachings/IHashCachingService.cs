namespace UserService.Application.Interfaces.Cachings;

public interface IHashCachingService
{
    Task SetAsync<T>(string key, T value);
    Task SetFieldAsync<T>(string key, string field, T? value);
    Task<Dictionary<string, string?>?> GetFieldAsync(string key, params string[] field);
    Task<Dictionary<string, string>?> GetAsync(string key);
    Task<T?> GetAsync<T>(string key) where T : class, new();
    Task<bool> RemoveFieldAsync(string key, string field);
    Task<bool> RemoveAsync(string key);
    Task<bool> KeyExistsAsync(string key);
}
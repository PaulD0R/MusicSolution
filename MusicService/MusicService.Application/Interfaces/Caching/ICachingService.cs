namespace MusicService.Application.Interfaces.Caching;

public interface ICachingService
{
    Task<bool> SetAsync<T>(string key, T value, TimeSpan ttl) where T : class;
    Task<T?> GetAsync<T>(string key) where T : class;
    Task<bool> RemoveAsync(string key);
}
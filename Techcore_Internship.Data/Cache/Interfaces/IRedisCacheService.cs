namespace Techcore_Internship.Data.Cache.Interfaces;

public interface IRedisCacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}

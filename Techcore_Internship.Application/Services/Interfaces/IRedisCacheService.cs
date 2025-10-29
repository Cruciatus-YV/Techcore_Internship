namespace Techcore_Internship.Application.Services.Interfaces;

public interface IRedisCacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}

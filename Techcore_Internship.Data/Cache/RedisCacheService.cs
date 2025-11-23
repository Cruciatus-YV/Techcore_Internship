using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Techcore_Internship.Data.Cache.Interfaces;
using Techcore_Internship.Contracts.Configurations;

namespace Techcore_Internship.Data.Cache;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;
    private readonly RedisSettings _redisSettings;

    public RedisCacheService(IDistributedCache cache, IOptions<RedisSettings> redisSettings)
    {
        _cache = cache;
        _redisSettings = redisSettings.Value;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var fullKey = $"{_redisSettings.InstanceName}{key}";
        var cached = await _cache.GetStringAsync(fullKey);

        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<T>(cached)!;

        var result = await factory();
        var serialized = JsonSerializer.Serialize(result);

        await _cache.SetStringAsync(fullKey, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(_redisSettings.DefaultExpirationMinutes)
        });

        return result;
    }

    public async Task RemoveAsync(string key)
    {
        var fullKey = $"{_redisSettings.InstanceName}{key}";
        await _cache.RemoveAsync(fullKey);
    }
}

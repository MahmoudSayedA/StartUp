using Application.Common.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Services.Caching;
public class RedisCacheService(IDistributedCache cache) : ICacheService
{
    private readonly IDistributedCache _cache = cache;

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken)
    {
        var data = await _cache.GetStringAsync(key);
        if (data is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetDataAsync<T>(string key, T data, CancellationToken cancellationToken)
    {
        var option = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), option, cancellationToken);

    }

    public async Task<long> GetVersionAsync(string masterKey, CancellationToken cancellationToken)
    {
        return await GetDataAsync<long>($"{masterKey}:version", cancellationToken);
    }
    public async Task IncrementVersionAsync(string masterKey, CancellationToken cancellationToken)
    {
        long version = await GetVersionAsync(masterKey, cancellationToken);
        version++;

        var key = $"{masterKey}:version";

        await SetDataAsync(key, version, cancellationToken);
    }


}

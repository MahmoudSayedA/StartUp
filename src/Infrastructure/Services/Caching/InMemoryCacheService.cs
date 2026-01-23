using Application.Common.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Infrastructure.Services.Caching;
public class InMemoryCacheService(IMemoryCache cache) : ICacheService
{
    private readonly IMemoryCache _cache = cache;

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        await Task.Run(() => _cache.Remove(key), cancellationToken);
    }

    public async Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken)
    {
        var data = await Task.Run(() => _cache.Get(key), cancellationToken) as string;
        if (data is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetDataAsync<T>(string key, T data, CancellationToken cancellationToken)
    {

        await Task.Run(() => _cache.Set(key, JsonSerializer.Serialize(data)), cancellationToken);

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

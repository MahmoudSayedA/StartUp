using Application.Common.Abstractions.Caching;

namespace Application.Unit.Tests.common.Abstraction;
internal class FakeCacheService : ICacheService
{
    private readonly Dictionary<string, object?> _cache = new();
    private readonly Dictionary<string, long> _versions = new();

    public Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken)
    {
        _cache.TryGetValue(key, out var value);
        return Task.FromResult((T?)value);
    }

    public Task<long> GetVersionAsync(string masterKey, CancellationToken cancellationToken)
    {
        _versions.TryGetValue(masterKey, out var version);
        return Task.FromResult(version);
    }

    public Task IncrementVersionAsync(string masterKey, CancellationToken cancellationToken)
    {
        if (_versions.ContainsKey(masterKey))
        {
            _versions[masterKey]++;
        }
        else
        {
            _versions[masterKey] = 1;
        }
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task SetDataAsync<T>(string key, T data, CancellationToken cancellationToken)
    {
        _cache[key] = data;
        return Task.CompletedTask;
    }
}

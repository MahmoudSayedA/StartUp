using Application.Common.Abstractions.Caching;
using Microsoft.Extensions.Logging;
using Polly;
using StackExchange.Redis;

namespace Infrastructure.Services.Caching;

public class ResilientCacheService : ICacheService
{
    private readonly RedisCacheService _cacheService;
    private readonly IAsyncPolicy _circuitBreakerPolicy;
    private readonly ILogger<ResilientCacheService> _logger;
    public ResilientCacheService(RedisCacheService cacheService, ILogger<ResilientCacheService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;

        _circuitBreakerPolicy = Policy
            .Handle<RedisConnectionException>()
            .Or<RedisTimeoutException>()
            .Or<TimeoutException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 1,
                durationOfBreak: TimeSpan.FromSeconds(60),
                onBreak: (ex, breakDelay) => _logger.LogWarning("Cache service circuit breaker opened for {BreakDelay} due to: {ExceptionMessage}", breakDelay, ex.Message),
                onReset: () => _logger.LogInformation("Cache service circuit breaker reset.")
            );
    }

    public async Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken)
    {
        try
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => 
                await _cacheService.GetDataAsync<T>(key, cancellationToken));
        }
        catch (Exception)
        {

            _logger.LogWarning("Failed to get data from cache for key: {Key}", key);
            return default;
        }
    }

    public async Task SetDataAsync<T>(string key, T data, CancellationToken cancellationToken)
    {
        try
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () => 
                await _cacheService.SetDataAsync<T>(key, data, cancellationToken));
        }
        catch (Exception)
        {
            _logger.LogWarning("Failed to set data in cache for key: {Key}", key);
        }
    }

    public async Task<long> GetVersionAsync(string masterKey, CancellationToken cancellationToken)
    {
        try
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => 
                await _cacheService.GetVersionAsync(masterKey, cancellationToken));
        }
        catch (Exception)
        {
            _logger.LogWarning("Failed to get version from cache for master key: {MasterKey}", masterKey);
            return await Task.FromResult(0L);
        }
    }

    public async Task IncrementVersionAsync(string masterKey, CancellationToken cancellationToken)
    {
        try
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () => 
                await _cacheService.IncrementVersionAsync(masterKey, cancellationToken));
        }
        catch (Exception)
        {

            _logger.LogWarning("Failed to increment version in cache for master key: {MasterKey}", masterKey);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () => 
                await _cacheService.RemoveAsync(key, cancellationToken));
        }
        catch (Exception)
        {

            _logger.LogWarning("Failed to remove data from cache for key: {Key}", key);
        }
    }

}

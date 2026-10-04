using RepositorioRemoto.Back.Cache.Common;
using Serilog;

namespace RepositorioRemoto.Back.Cache;

using Microsoft.Extensions.Caching.Memory;

public class MemoryCache(IMemoryCache cache) : ICache
{
    private readonly ILogger _logger = Log.ForContext<MemoryCache>();
    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = cache.Get<T>(key);
            return Task.FromResult(value);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Error obteniendo de caché. Clave={Key}", key);
            return Task.FromResult(default(T));
        }
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            }

            cache.Set(key, value, options);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Error estableciendo caché. Clave={Key}", key);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key)
    {
        try
        {
            cache.Remove(key);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Error eliminando de caché. Clave={Key}", key);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public Task RemoveAllAsync()
    {
        try
        {
            if (cache is Microsoft.Extensions.Caching.Memory.MemoryCache memoryCache)
            {
                memoryCache.Clear();
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Error limpiando la caché en memoria.");
            return Task.CompletedTask;
        }
    }
}
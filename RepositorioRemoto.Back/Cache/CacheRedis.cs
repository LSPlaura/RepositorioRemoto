using RepositorioRemoto.Back.Cache.Common;
using Serilog;

namespace RepositorioRemoto.Back.Cache;

using System.Text.Json;
using StackExchange.Redis;

public class RedisCache(IConnectionMultiplexer redis) : ICache
{
    private readonly ILogger _logger = Log.ForContext<RedisCache>();

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var db = redis.GetDatabase();
            var value = await db.StringGetAsync(key);

            if (value.IsNull)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>((string)value!);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Error obteniendo de caché Redis. Clave={Key}", key);
            return default;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var db = redis.GetDatabase();
            var serializedValue = JsonSerializer.Serialize(value);
            var ttl = expiration ?? TimeSpan.FromMinutes(5);

            await db.StringSetAsync(key, serializedValue, ttl);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Error estableciendo caché Redis. Clave={Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key)
    {
        try
        {
            var db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Error eliminando de caché Redis. Clave={Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task RemoveAllAsync()
    {
        try
        {
            var database = redis.GetDatabase();
            var endpoints = redis.GetEndPoints();

            foreach (var endpoint in endpoints)
            {
                var server = redis.GetServer(endpoint);

                if (server.IsReplica)
                {
                    continue;
                }

                var keys = server.Keys(database.Database).ToArray();

                if (keys.Length > 0)
                {
                    await database.KeyDeleteAsync(keys);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error limpiando la caché en Redis.");
            throw;
        }
    }
}
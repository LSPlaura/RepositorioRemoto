using System.Text.Json;
using RepositorioRemoto.Back.Cache.Common;
using Serilog;
using StackExchange.Redis;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace RepositorioRemoto.Back.Cache;

/// <summary>
/// Implementación del patrón Cache-Aside mediante Redis.
/// 
/// Se consulta la caché. Si el dato no está disponible,
/// se obtiene de la fuente de datos y se almacena en Redis.
/// </summary>
/// <param name="redis">Multiplexor de conexión a Redis.</param>
public class Cache(IConnectionMultiplexer redis) : ICache
{
    private readonly IDatabase _db = redis.GetDatabase();
    private readonly Serilog.ILogger _logger = Log.ForContext<Cache>();

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        _logger.Debug("Consultando la caché para la clave {Key}", key);

        //se obtiene el json mediante la clave
        var value = await _db.StringGetAsync(key);

        //se comprueba que no sea nulo
        if (value.IsNullOrEmpty)
        {
            _logger.Debug("No se encontró la clave {Key} en la caché", key);
            return null;
        }

        _logger.Debug("Se encontró la clave {Key} en la caché", key);

        //se deserializa al tipo que se especifique pero primero se pasa a string (con (string))
        //ya que StringGetAsync devuelve un RedisValue
        return JsonSerializer.Deserialize<T>((string)value!);
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        _logger.Debug("Guardando la clave {Key} en la caché", key);

        //se serializa el obj
        var json = JsonSerializer.Serialize(value);
        
        //si no es nulo se utiliza el tiempo de expiración
        if (expiration.HasValue)
        {
            await _db.StringSetAsync(key, json, expiration.Value);
            _logger.Debug("Clave {Key} guardada en la caché con TTL de {Expiration}", key, expiration.Value);
        }
        else //si es nulo redis garda sin TTL (Time To Live, tiempo de vida)
        {
            await _db.StringSetAsync(key, json);
            _logger.Debug("Clave {Key} guardada en la caché sin expiración", key);
        }
    }

    /// <inheritdoc />
    public async Task<bool> RemoveAsync(string key)
    {
        _logger.Debug("Eliminando la clave {Key} de la caché", key);

        //se borra la clave y devuelve valor booleano
        var result = await _db.KeyDeleteAsync(key);

        _logger.Debug("Resultado de eliminación de la clave {Key}: {Result}", key, result);

        return result;
    }
}

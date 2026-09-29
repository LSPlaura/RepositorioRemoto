namespace RepositorioRemoto.Back.Cache.Common;

/// <summary>
/// Interfaz para operaciones de caché genéricas.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Obtiene un valor de la caché por su clave.
    /// </summary>
    /// <typeparam name="T">Tipo del valor a deserializar.</typeparam>
    /// <param name="key">Clave de búsqueda.</param>
    /// <returns>El valor deserializado o null si no existe.</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Almacena un valor en la caché con una caducidad opcional.
    /// </summary>
    /// <typeparam name="T">Tipo del valor a serializar.</typeparam>
    /// <param name="key">Clave de almacenamiento.</param>
    /// <param name="value">Valor a guardar.</param>
    /// <param name="expiration">Tiempo hasta la caducidad. Null = sin caducidad.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Elimina un valor de la caché por su clave.
    /// </summary>
    /// <param name="key">Clave a eliminar.</param>
    /// <returns>True si se eliminó, false si no existía.</returns>
    Task<bool> RemoveAsync(string key);
}
using Microsoft.Extensions.Configuration;

namespace RepositorioRemoto.Back.Config;

/// <summary>
/// Lee la configuración del archivo correspondiente al entorno
/// indicado en los argumentos del programa.
/// </summary>
public static class Configuracion {
    /// <summary>
    /// Obtiene la configuración cargada.
    /// </summary>
    public static IConfiguration Configuration { get; private set; } = null!;
    

    /// <summary>
    /// Carga el archivo de configuración según el primer argumento.
    /// Si no se proporciona, utiliza Development.
    /// </summary>
    /// <param name="args">Argumentos recibidos en Main.</param>
    /// <exception cref="ArgumentException">
    /// El entorno indicado no es Development ni Production.
    /// </exception>
    public static void Inicializar(string[] args) {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile($"appsettings.{ApiName}.json", false, true)
            .Build();
    }

    /// <summary>
    /// Obtiene el nombre definido en la configuración de la API.
    /// </summary>
    public static string ApiName => Configuration.GetValue<string>("ApiSettings:Name") ?? "Development";

    /// <summary>
    /// Obtiene la URL base de la API REST.
    /// </summary>
    public static string BaseUrl => Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://jsonplaceholder.typicode.com";

    /// <summary>
    /// Obtiene el nombre del repositorio configurado.
    /// </summary>
    public static string RepositoryName => Configuration.GetValue<string>("Repository:Name") ?? "SQLite";

    /// <summary>
    /// Obtiene el nombre del proveedor de caché configurado.
    /// </summary>
    public static string CacheName => Configuration.GetValue<string>("Cache:Name") ?? "Memory";

    /// <summary>
    /// Obtiene el tiempo de vida configurado para los elementos de la caché.
    /// </summary>
    public static int CacheTtl => Configuration.GetValue("Cache:TTL", 30);

    /// <summary>
    /// Obtiene el intervalo de sincronización en segundos.
    /// </summary>
    public static int CacheSincronizacion => Configuration.GetValue("Cache:Sincronización", 60);
}
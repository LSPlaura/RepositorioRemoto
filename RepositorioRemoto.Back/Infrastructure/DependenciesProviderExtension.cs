using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RepositorioRemoto.Back.Infrastructure;

public static class DependenciesProviderExtension
{
    /// <summary>
    /// Registra el DbContext según el entorno (PostgreSQL en Producción, SQLite en Desarrollo).
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            if (config.config.Estado = "Prod")
            {
                Log.Information("Configurando PostgreSQL para Producción...");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "ConnectionStrings:DefaultConnection no está definida. " +
                        "En producción es obligatorio configurarla (appsettings.json o variables de entorno).");
                }

                options.UseNpgsql(connectionString);
            }
            else
            {
                Log.Information("Configurando SQLite para Desarrollo...");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    connectionString = "Data Source=local_database.db";
                    Log.Warning("Usando SQLite por defecto para desarrollo: {ConnectionString}", connectionString);
                }

                options.UseSqlite(connectionString);
            }
        });

        return services;
    }

    /// <summary>
    /// Registra la caché según el entorno (Redis en Producción, MemoryCache en Desarrollo).
    /// </summary>
    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        if (config.config.Estado = "Prod")
        {
            Log.Information("Configurando caché con Redis para Producción...");

            var redisConnectionString = config.config.CacheConnectionString("Redis");

            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new InvalidOperationException(
                    "ConnectionStrings:Redis no está definida. " +
                    "En producción es obligatorio configurar la conexión a Redis.");
            }

            services.AddRedisCacheService(redisConnectionString);
        }
        else
        {
            Log.Information("Configurando caché en memoria (MemoryCache) para Desarrollo...");
            services.AddMemoryCacheService();
        }
        return services;
    }
}
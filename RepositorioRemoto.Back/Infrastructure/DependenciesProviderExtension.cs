using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Back.Cache;
using RepositorioRemoto.Back.Cache.Common;
using RepositorioRemoto.Back.Entity;
using Serilog;

namespace RepositorioRemoto.Back.Infrastructure;

public static class DependenciesProviderExtension {
    /// <summary>
    /// Registra el DbContext según el entorno en config.config.
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services) {
        if (config.config.Estado == "Prod") {
            Log.Information("Configurando PostgreSQL para Producción...");
            services.AddDbContext<AppDbContextPostgre>(options => {
                options.UseNpgsql(config.config.DbConnection);
            });
        } else {
            Log.Information("Configurando SQLite para Desarrollo...");
            services.AddDbContext<AppDbContextSqlite>(options => {
                options.UseSqlite(config.config.DbConnection);
            });
        }

        return services;
    }

    /// <summary>
    /// Registra la caché según el entorno en config.config.
    /// </summary>
    public static IServiceCollection AddCache(this IServiceCollection services) {
        if (config.config.Estado == "Prod") {
            Log.Information("Configurando caché con Redis para Producción...");
            services.AddSingleton<ICache>(sp => new RedisCache(config.config.CacheConnectionString));
        } else {
            Log.Information("Configurando caché en memoria (MemoryCache) para Desarrollo...");
            services.AddMemoryCache();
            services.AddSingleton<ICache, MemoryCache>();
        }

        return services;
    }
}
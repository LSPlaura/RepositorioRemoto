using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Back.Cache;
using RepositorioRemoto.Back.Cache.Common;
using RepositorioRemoto.Back.Config;
using RepositorioRemoto.Back.Entity;
using StackExchange.Redis;
using Serilog;

namespace RepositorioRemoto.Back.Infrastructure;

public static class DependenciesProviderExtension {
    /// <summary>
    /// Registra el DbContext según el entorno en config.config.
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services) {
        if (Configuracion.ApiName == "Production") {
            Log.Information("Configurando PostgreSQL para Producción...");
            services.AddDbContext<AppDbContextPostgre>(options => {
                options.UseNpgsql(Configuracion.DbConnection);
            });
        } else {
            Log.Information("Configurando SQLite para Desarrollo...");
            services.AddDbContext<AppDbContextSqlite>(options => {
                options.UseSqlite(Configuracion.DbConnection);
            });
        }
        return services;
    }

    /// <summary>
    /// Registra la caché según el entorno en config.config.
    /// </summary>
    public static IServiceCollection AddCache(this IServiceCollection services) {
        if (Configuracion.ApiName == "Production") {
            Log.Information("Configurando caché con Redis para Producción...");
            services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(Configuracion.CacheConnectionString));
            services.AddSingleton<ICache, RedisCache>();
        } else {
            Log.Information("Configurando caché en memoria (MemoryCache) para Desarrollo...");
            services.AddMemoryCache();
            services.AddSingleton<ICache, MemoryCache>();
        }
        return services;
    }
}
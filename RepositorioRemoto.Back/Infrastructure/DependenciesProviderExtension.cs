using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using RepositorioRemoto.Back.Api;
using RepositorioRemoto.Back.Cache;
using RepositorioRemoto.Back.Cache.Common;
using RepositorioRemoto.Back.Config;
using RepositorioRemoto.Back.Entity;
using StackExchange.Redis;
using Serilog;

namespace RepositorioRemoto.Back.Infrastructure;

public static class DependenciesProviderExtension {
    /// <summary>
    /// Registra únicamente el DbContext correspondiente al entorno.
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services) {
        if (Configuracion.ApiName == "Production") 
        {
            Log.Information("Configurando PostgreSQL para Producción...");
            
            services.AddDbContext<AppDbContextPostgre>(options => {
                options.UseNpgsql(Configuracion.DbConnection);
            });

            // Registra DbContext apuntando al contexto de PostgreSQL
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContextPostgre>());
        } 
        else 
        {
            Log.Information("Configurando SQLite para Desarrollo / Pruebas...");

            // Mantener la conexión abierta para SQLite
            var connection = new SqliteConnection(Configuracion.DbConnection);
            connection.Open();
            services.AddSingleton(connection);

            services.AddDbContext<AppDbContextSqlite>(options => {
                options.UseSqlite(connection);
            });

            // Registra DbContext apuntando al contexto de SQLite
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContextSqlite>());
        }

        return services;
    }

    /// <summary>
    /// Método de extensión para inicializar las tablas de la BD usando el contenedor.
    /// </summary>
    public static IServiceProvider InitializeDatabase(this IServiceProvider provider) {
        using var scope = provider.CreateScope();
        
        // Obtiene la interfaz DbContext (que resolverá la adecuada según el entorno)
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();
        context.Database.EnsureCreated();

        return provider;
    }

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
    
    /// <summary>
    /// Registra los clientes y servicios de API externos.
    /// </summary>
    public static IServiceCollection AddExternalApis(this IServiceCollection services) {
        services.AddSingleton<IApiJsonPlaceHolder>(_ => {
            var client = new HttpClient {
                BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
            };

            return RestService.For<IApiJsonPlaceHolder>(client);
        });

        return services;
    }
}
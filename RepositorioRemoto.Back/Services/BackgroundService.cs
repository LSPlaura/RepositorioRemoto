using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Back.Api;
using RepositorioRemoto.Back.Cache.Common;
using RepositorioRemoto.Back.Repositories;
using Serilog;

namespace RepositorioRemoto.Back.Services;

/// <summary>
/// Servicio para la ejecución periódica de sincronización de usuarios en segundo plano.
/// </summary>
public class BackgroundService(IServiceProvider provider, ICache cache)
{
    private readonly ILogger _logger = Log.ForContext<BackgroundService>();

    /// <summary>
    /// Inicia el bucle de sincronización periódica cada 60 segundos.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la ejecución del servicio.</param>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                _logger.Information("Iniciando ciclo de sincronización de usuarios...");

                using var scope = provider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var api = scope.ServiceProvider.GetRequiredService<IApiJsonPlaceHolder>();

                await Synchronize(repository, api);

                _logger.Information("Sincronización completada con éxito.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error durante el ciclo de sincronización de usuarios.");
            }
        }
    }

    /// <summary>
    /// Realiza la limpieza de caché, liempieza de la base de datos e inserción de los usuarios de la API.
    /// </summary>
    /// <param name="repository">Repositorio para la gestión de usuarios en base de datos.</param>
    /// <param name="api">Cliente de la API remota para la obtención de datos.</param>
    private async Task Synchronize(IUserRepository repository, IApiJsonPlaceHolder api)
    {
        await cache.RemoveAllAsync();
        await repository.DeleteAllAsync();

        var users = await api.GetUserAsync();

        foreach (var user in users)
        {
            await repository.CreateAsync(user);
        }
    }
}
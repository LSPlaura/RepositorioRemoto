namespace RepositorioRemoto.Back.Services.Background;

/// <summary>
/// Define la abstracción para servicios que ejecutan tareas en segundo plano.
/// </summary>
public interface IBackgroundService
{
    /// <summary>
    /// Inicia la ejecución asíncrona de la tarea en segundo plano.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token de cancelación para notificar cuando el servicio debe detenerse de forma limpia.
    /// </param>
    /// <returns>Una tarea que representa la operación de ejecución del servicio.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);
}
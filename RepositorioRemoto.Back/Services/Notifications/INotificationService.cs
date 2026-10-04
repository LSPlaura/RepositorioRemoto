using RepositorioRemoto.Back.Models.Notification;

namespace RepositorioRemoto.Back.Notifications;

/// <summary>
/// Define el contrato del servicio de emisión y suscripción a notificaciones.
/// </summary>
public interface INotificationService {
    
    /// <summary>
    /// Obtiene el flujo observable al que pueden suscribirse los consumidores
    /// para recibir notificaciones.
    /// </summary>
    /// <value>Flujo de eventos de tipo.</value>
    IObservable<Notification> Observable { get; }

    /// <summary>
    /// Emite una notificación a los suscriptores del servicio.
    /// </summary>
    /// <param name="notificacion">Notificación que se desea emitir.</param>
    void Notificar(Notification notificacion);
}
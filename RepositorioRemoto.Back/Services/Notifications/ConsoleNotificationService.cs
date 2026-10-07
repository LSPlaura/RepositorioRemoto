using System.Reactive.Linq;
using System.Reactive.Subjects;
using RepositorioRemoto.Back.Infrastructure.Interfaces;
using RepositorioRemoto.Back.Models.Notification;
using RepositorioRemoto.Back.Notifications;

namespace RepositorioRemoto.Back.Services.Notifications;

/// <summary>
/// Servicio que emite notificaciones a través de un flujo observable.
/// </summary>
public class ConsoleNotificationService : INotificationService, IDisposable, ISingletonService {
    /// <summary>
    /// Emisor de notificaciones que las distribuye a los suscriptores activos.
    /// </summary>
    private readonly Subject<Notification> _subject = new();

    /// <inheritdoc cref="INotificationService.Observable" />
    public IObservable<Notification> Observable => _subject.AsObservable();

    /// <inheritdoc cref="INotificationService.Notificar" />
    public void Notificar(Notification notificacion) {
        _subject.OnNext(notificacion);
    }

    public void Dispose() {
        _subject.OnCompleted();
        _subject.Dispose();
    }
}
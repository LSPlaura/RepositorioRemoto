using RepositorioRemoto.Back.Enum;

namespace RepositorioRemoto.Back.Models.Notification;

/// <summary>
/// Representa una notificación sobre una operación realizada con un usuario.
/// </summary>
public record Notification(
    TypeNotification Tipo,
    string Mensaje,
    DateTime Timestamp
);
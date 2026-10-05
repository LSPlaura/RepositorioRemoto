namespace RepositorioRemoto.Back.Errors.Service;

/// <summary>
/// Representa los errores relacionados con la gestión de usuarios.
/// </summary>
public abstract record ServiceError(string Message) : DomainError(Message) {
    /// <summary>
    /// Error cuando no se encuentra el usuario solicitado.
    /// </summary>
    public sealed record NotFoundError(int Id)
        : ServiceError($"No se ha encontrado el usuario con el id: {Id}");

    /// <summary>
    /// Error de validación de un campo del usuario.
    /// </summary>
    public sealed record ValidationError(string Field, string Message)
        : ServiceError(Message);

    /// <summary>
    /// Error al obtener todos los usuarios.
    /// </summary>
    public sealed record GetAllError(string Detail)
        : ServiceError($"Error al obtener los usuarios: {Detail}");

    /// <summary>
    /// Error al obtener un usuario por su identificador.
    /// </summary>
    public sealed record GetByIdError(int Id, string Detail)
        : ServiceError($"Error al obtener el usuario con el id {Id}: {Detail}");

    /// <summary>
    /// Error al crear un usuario.
    /// </summary>
    public sealed record CreateError(string Detail)
        : ServiceError($"Error al crear el usuario: {Detail}");

    /// <summary>
    /// Error al actualizar un usuario.
    /// </summary>
    public sealed record UpdateError(int Id, string Detail)
        : ServiceError($"Error al actualizar el usuario con el id {Id}: {Detail}");

    /// <summary>
    /// Error al eliminar un usuario.
    /// </summary>
    public sealed record DeleteError(int Id, string Detail)
        : ServiceError($"Error al eliminar el usuario con el id {Id}: {Detail}");

    /// <summary>
    /// Error al exportar los usuarios en formato JSON.
    /// </summary>
    public sealed record ExportToJsonError(string Detail)
        : ServiceError($"Error al exportar los usuarios a JSON: {Detail}");
}

/// <summary>
/// Proporciona métodos para crear los errores de gestión de usuarios.
/// </summary>
public static class ServiceErrors {
    public static DomainError NotFoundError(int id) {
        return new ServiceError.NotFoundError(id);
    }

    public static DomainError ValidationError(string field, string message) {
        return new ServiceError.ValidationError(field, message);
    }

    public static DomainError GetAllError(string detail) {
        return new ServiceError.GetAllError(detail);
    }

    public static DomainError GetByIdError(int id, string detail) {
        return new ServiceError.GetByIdError(id, detail);
    }

    public static DomainError CreateError(string detail) {
        return new ServiceError.CreateError(detail);
    }

    public static DomainError UpdateError(int id, string detail) {
        return new ServiceError.UpdateError(id, detail);
    }

    public static DomainError DeleteError(int id, string detail) {
        return new ServiceError.DeleteError(id, detail);
    }

    public static DomainError ExportToJsonError(string detail) {
        return new ServiceError.ExportToJsonError(detail);
    }
}
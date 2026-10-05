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
    /// Error al obtener todos los usuarios.
    /// </summary>
    public sealed record GetAllError()
        : ServiceError("Error al obtener los usuarios.");

    /// <summary>
    /// Error al obtener un usuario por su identificador.
    /// </summary>
    public sealed record GetByIdError(int Id)
        : ServiceError("Error al obtener el usuario con el id {Id}.");

    /// <summary>
    /// Error al crear un usuario.
    /// </summary>
    public sealed record CreateError()
        : ServiceError("Error al crear el usuario.}");

    /// <summary>
    /// Error al actualizar un usuario.
    /// </summary>
    public sealed record UpdateError(int Id)
        : ServiceError($"Error al actualizar el usuario con el id {Id}.");

    /// <summary>
    /// Error al eliminar un usuario.
    /// </summary>
    public sealed record DeleteError(int Id)
        : ServiceError($"Error al eliminar el usuario con el id {Id}.");

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

    public static DomainError GetAllError() {
        return new ServiceError.GetAllError();
    }

    public static DomainError GetByIdError(int id) {
        return new ServiceError.GetByIdError(id);
    }

    public static DomainError CreateError() {
        return new ServiceError.CreateError();
    }

    public static DomainError UpdateError(int id) {
        return new ServiceError.UpdateError(id);
    }

    public static DomainError DeleteError(int id) {
        return new ServiceError.DeleteError(id);
    }

    public static DomainError ExportToJsonError(string detail) {
        return new ServiceError.ExportToJsonError(detail);
    }
}
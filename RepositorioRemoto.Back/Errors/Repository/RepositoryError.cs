namespace RepositorioRemoto.Back.Errors.Repository;

public abstract record RepositoryError(string Message) : DomainError(Message) {
    /// <summary>
    /// Error específico para el error en el registro de users.
    /// </summary>
    public sealed record CreationError() : RepositoryError($"No se ha podido registar el nuevo usuario en el sistema.");
    
    /// <summary>
    /// Error específico para el error en la actualizacion del user.
    /// </summary>
    public sealed record UpdatedError() : RepositoryError($"No se ha podido actualizar el usuario en el sistema.");
    
    /// <summary>
    /// Error específico para el error en la eliminacion del user.
    /// </summary>
    public sealed record DeletedError() : RepositoryError($"No se ha podido eliminar el usuario en el sistema.");
}

public static class RepositoryErrors {
    public static DomainError CreationError() {
        return new RepositoryError.CreationError();
    }
    
    public static DomainError UpdatedError() {
        return new RepositoryError.UpdatedError();
    }
    
    public static DomainError DeletedError() {
        return new RepositoryError.DeletedError();
    }    
}
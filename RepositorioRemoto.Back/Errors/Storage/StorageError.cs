namespace RepositorioRemoto.Back.Errors.Storage;

public abstract record StorageError(string Message) : DomainError(Message) {
    /// <summary>
    /// Error específico para los usuarios no encontrados.
    /// </summary>
    public sealed record WriteError(string message) : StorageError("Error al intentar escribir el json.");
}

public static class StorageErrors {
    public static DomainError WriteError(string message) {
        return new StorageError.WriteError(message);
    }
}
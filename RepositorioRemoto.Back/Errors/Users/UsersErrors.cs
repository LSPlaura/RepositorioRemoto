namespace RepositorioRemoto.Back.Errors.Users;

public abstract record UsersError(string Message) : DomainError(Message) {
    /// <summary>
    /// Error específico para los usuarios no encontrados.
    /// </summary>
    public sealed record NotFoundError(int Id) : UsersError($"No se ha encontrado la entidad con el id: {Id}");
    
    /// <summary>
    /// Error específico para la validacion.
    /// </summary>
    public sealed record ValidationError(string Field, string Message) : UsersError("Han surgido errores en la validacion de la entidad.");
}

public static class UsersErrors {
    public static DomainError NotFoundError(int Id) {
        return new UsersError.NotFoundError(Id);
    }
}
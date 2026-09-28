namespace RepositorioRemoto.Back.Errors.Users;

public record UsersErrors() : DomainError
{
    /// <summary>
    /// Error específico para los usuarios no encontrados.
    /// </summary>
    public sealed record NotFoundError(string Resource, int Id) : UsersErrors;
    
    /// <summary>
    /// Error específico para la validacion.
    /// </summary>
    public sealed record ValidationError(string Field, string Message) : UsersErrors;
}
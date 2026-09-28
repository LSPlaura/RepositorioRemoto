namespace RepositorioRemoto.Back.Error;

/// <summary>
/// Clase base para los errores del sistema.
/// </summary>
public abstract record DomainError {
    /// <summary>
    /// Error específico para los usuarios no encontrados.
    /// </summary>
    public sealed record NotFoundError(string Resource, int Id) : DomainError;
    
    /// <summary>
    /// Error específico para la validacion.
    /// </summary>
    public sealed record ValidationError(string Field, string Message) : DomainError;
    
    /// <summary>
    /// Error específico para la Api.
    /// </summary>
    public sealed record ApiError(int StatusCode, string Details) : DomainError;
}
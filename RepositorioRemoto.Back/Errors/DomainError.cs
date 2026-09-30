namespace RepositorioRemoto.Back.Errors;

/// <summary>
/// Clase base para los errores del sistema.
/// </summary>
public abstract record DomainError(string Message) { }
namespace RepositorioRemoto.Back.Errors.Api;

/// <summary>
/// Error específico para la Api.
/// </summary>
public sealed record ApiError(string Message) : DomainError(Message);
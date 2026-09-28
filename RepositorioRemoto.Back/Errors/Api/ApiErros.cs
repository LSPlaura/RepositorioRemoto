namespace RepositorioRemoto.Back.Errors.Api;

/// <summary>
/// Error específico para la Api.
/// </summary>
public sealed record ApiError(int StatusCode, string Details) : DomainError;
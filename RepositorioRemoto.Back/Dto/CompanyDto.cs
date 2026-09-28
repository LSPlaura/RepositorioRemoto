namespace RepositorioRemoto.Back.Dto;

/// <summary>
///  Representa el dto de una compañía dentro del sistema.
/// </summary>
public record CompanyDto(
    string Name,
    string CatchPhrase,
    string Bs
);
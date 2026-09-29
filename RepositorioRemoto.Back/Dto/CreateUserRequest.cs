namespace RepositorioRemoto.Back.Dto;

/// <summary>
/// Dto especializado para el registro de un nuevo usuario en el sistema.
/// </summary>
public record CreateUserRequest (
    string Name,
    string UserName,
    string Email,
    AddressDto Address,
    string Phone,
    string Website,
    CompanyDto Company
);
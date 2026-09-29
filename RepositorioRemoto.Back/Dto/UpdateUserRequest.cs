namespace RepositorioRemoto.Back.Dto;

/// <summary>
/// Dto especializado para editar el registro de un usuario ya registrado en el sistema.
/// </summary>
public record UpdateUserRequest (
    int Id,
    string Name,
    string UserName,
    string Email,
    AddressDto Address,
    string Phone,
    string Website,
    CompanyDto Company
);
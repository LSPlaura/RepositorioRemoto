using RepositorioRemoto.Back.Dtos;

namespace RepositorioRemoto.Back.Dtos;

/// <summary>
///  Representa el dto de un usuario dentro del sistema.
/// </summary>
public record UserDto(
    int Id,
    string Name,
    string UserName,
    string Email,
    AddressDto Address,
    string Phone,
    string Website,
    CompanyDto Company
);
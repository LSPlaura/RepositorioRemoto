namespace RepositorioRemoto.Back.Dto;

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
    CompanyDto Company,
    string CreateAt,
    string UpdateAt,
    string DeleteAt,
    bool IsDeleted
);
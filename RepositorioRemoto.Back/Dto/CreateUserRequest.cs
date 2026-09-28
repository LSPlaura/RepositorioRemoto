namespace RepositorioRemoto.Back.Dto;

/// <summary>
/// Dto especializado para el registro de un nuevo usuario en el sistema.
/// </summary>
public record CreateUserRequest (
    string Name,
    string UserName,
    string Email,
    string Address,
    string Phone,
    string Website,
    string Company,
    string CreateAt,
    string UpdateAt,
    string DeleteAt,
    bool IsDeleted
);
using RepositorioRemoto.Back.Dto.Users.Request;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Mappers;

/// <summary>
/// Contiene las funciones para convertir peticiones de usuario al modelo de dominio.
/// </summary>
public static class UserMapper {
    
    /// <summary>
    /// Convierte una petición de actualización a un usuario,
    /// conservando sus datos de creación y borrado.
    /// </summary>
    /// <param name="dto">Petición con los nuevos datos.</param>
    /// <param name="actual">Usuario existente antes de la actualización.</param>
    /// <returns>Usuario con los datos actualizados.</returns>
    public static User ToModel(this UpdateUserRequest dto) {
        return new User {
            Id = dto.Id,
            Name = dto.Name,
            UserName = dto.UserName,
            Email = dto.Email,
            Address = dto.Address.ToModel(),
            Phone = dto.Phone,
            Website = dto.Website,
            Company = dto.Company.ToModel(),
            UpdateAt = DateTime.UtcNow,

        };
    }
    
    /// <summary>
    /// Convierte una petición de creación al modelo de dominio User.
    /// </summary>
    /// <param name="dto">Petición con los datos del nuevo usuario.</param>
    /// <returns>Usuario nuevo sin identificador asignado.</returns>
    public static User ToModel(this CreateUserRequest dto) {
        return new User {
            Id = 0,
            Name = dto.Name,
            UserName = dto.UserName,
            Email = dto.Email,
            Address = dto.Address.ToModel(),
            Phone = dto.Phone,
            Website = dto.Website,
            Company = dto.Company.ToModel(),
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
            DeleteAt = default,
            IsDeleted = false
        };
    }
}
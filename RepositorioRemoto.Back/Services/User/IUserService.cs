using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Errors;
using UserModel = RepositorioRemoto.Back.Models.User;

namespace RepositorioRemoto.Back.Services.User;

/// <summary>
/// Define las operaciones de gestión de usuarios.
/// </summary>
/// 
public interface IUserService {
    
    /// <summary>
    /// Obtiene los usuarios de la base de datos local.
    /// Si está vacía, los obtiene de la API y los almacena.
    /// </summary>
    /// <returns>Usuarios encontrados o un error.</returns>
    Task<IEnumerable<UserModel>> GetAllAsync();

    /// <summary>
    /// Busca un usuario en caché, base de datos y API, en ese orden.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <returns>Usuario encontrado o un error.</returns>
    Task<Result<UserModel, DomainError>> GetByIdAsync(int id);

    /// <summary>
    /// Válida y crea un usuario en la API, base de datos y caché.
    /// Emite una notificación cuando se completa la operación.
    /// </summary>
    /// <param name="request">Datos del nuevo usuario.</param>
    /// <returns>Usuario creado o un error.</returns>
    Task<Result<UserModel, DomainError>> CreateAsync(CreateUserRequest request);

    /// <summary>
    /// Válida y actualiza un usuario en la API, base de datos y caché.
    /// Emite una notificación cuando se completa la operación.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="request">Nuevos datos del usuario.</param>
    /// <returns>Usuario actualizado o un error.</returns>
    Task<Result<UserModel, DomainError>> UpdateAsync(int id, UpdateUserRequest request);

    /// <summary>
    /// Elimina un usuario de la API, base de datos y caché.
    /// Emite una notificación cuando se completa la operación.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <returns>Usuario eliminado o un error.</returns>
    Task<Result<UserModel, DomainError>> DeleteAsync(int id);

    /// <summary>
    /// Obtiene todos los usuarios y delega su escritura JSON en el almacenamiento.
    /// </summary>
    /// <returns>Ruta del archivo generado o un error.</returns>
    Task<Result<bool, DomainError>> ExportToJsonAsync();
}
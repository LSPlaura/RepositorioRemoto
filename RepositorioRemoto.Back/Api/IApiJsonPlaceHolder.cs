using Refit;
using RepositorioRemoto.Back.Dto;
using RepositorioRemoto.Back.Dto.Users.Request;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Api;

/// <summary>
/// Interfaz que define los endpoints para al Api de JsonPlaceHolder.
/// Utiliza Refit para generar la implementacion en tiempo de compilacion.
/// </summary>

[Headers("Content-Type: application/json")]
public interface IApiJsonPlaceHolder {
    
    /// <summary>
    /// Obtiene todos los usuarios.
    /// </summary>
    /// <returns>Lista con todos los usuarios.</returns>
    [Get("/users")]
    Task<List<User>> GetUserAsync();
    
    /// <summary>
    /// Obtiene un usuario en base a un id.
    /// </summary>
    /// <param name="id">Id del usuario a buscar.</param>
    /// <returns>Usuario si lo encuentra y en caso contrario nulo.</returns>
    [Get("/users/{id}")]
    Task<User?> GetUserByIdAsync(int id);
    
    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    /// <param name="request">Nuevo usuario.</param>
    /// <returns>Usuario creado.</returns>
    [Post("/users")]
    Task<User> CreateUserAsync([Body] CreateUserRequest request);
    
    /// <summary>
    /// Edita un usuario ya registrado en el sistema en base a su id.
    /// </summary>
    /// <param name="id">Id del usuario a editar.</param>
    /// <param name="request">Nuevo usuario.</param>
    /// <returns>Usuario actualizado.</returns>
    [Put("/users/{id}")]
    Task<User> UpdateUserAsync(int id, [Body] UpdateUserRequest request);

    /// <summary>
    /// Elimina un usuario ya registrado en el sistema en base al id.
    /// </summary>
    /// <param name="id">Id del usuario a eliminar.</param>
    [Delete("/users/{id}")]
    Task DeleteUserAsync(int id);
}
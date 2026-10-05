using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Storage;

/// <summary>
/// Define el contrato para la exportación de usuarios en formato JSON.
/// </summary>
public interface IUserStorage {
    
    /// <summary>
    /// Exporta de forma asíncrona todos los usuarios proporcionados a un archivo JSON.
    /// </summary>
    /// <param name="items">Colección completa de usuarios que se desea exportar.</param>
    /// <param name="path">Ruta del archivo JSON de destino.</param>
    /// <returns>
    /// Una tarea con el resultado de la exportación: un valor booleano que indica si se ha completado correctamente o un error de dominio en caso de fallo.
    /// </returns>
    Task<Result<bool, DomainError>> ExportarJsonAsync(IEnumerable<User> items, string path);
}
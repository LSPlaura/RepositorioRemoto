using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Errors;
namespace RepositorioRemoto.Back.Validator;

/// <summary>
/// Interfaz genérica para inversión de dependencias
/// </summary>
public interface IValidate<T> {
    /// <summary>
    /// Implementa las validaciones necesarias y en caso de no cumplir con ellas
    /// retorna un error
    /// </summary>
    /// <param name="item">Instancia a validar</param>
    Result<bool, DomainError> Validate(T item);
}
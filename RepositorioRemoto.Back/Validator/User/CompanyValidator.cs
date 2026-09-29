using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Users;

namespace RepositorioRemoto.Back.Validator.User;

/// <summary>
/// Validador para <see cref="CompanyDto"/>
///como método de extensión para que varios validadores lo puedan utilizar
/// </summary>
public static class CompanyValidator 
{
    public static Result<bool, DomainError> CheckEmptyOrWhiteSpace(this CompanyDto item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
            return new UsersErrors.ValidationError();
        
        if (string.IsNullOrWhiteSpace(item.Bs))
            return new UsersErrors.ValidationError();
        
        if (string.IsNullOrWhiteSpace(item.CatchPhrase))
            return new UsersErrors.ValidationError();

        return true;
    }
}
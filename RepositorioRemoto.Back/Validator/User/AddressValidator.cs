using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Users;

namespace RepositorioRemoto.Back.Validator.User;

/// <summary>
/// Validador para <see cref="AddressDto"/>
///como clase estática para que varios validadores lo puedan utilizar
/// </summary>
public static class AddressValidator
{
    public static Result<bool, DomainError> CheckEmptyOrWhiteSpace(this AddressDto item)
    {
        if (string.IsNullOrWhiteSpace(item.Suite))
            return new UsersErrors.ValidationError();
        
        if (string.IsNullOrWhiteSpace(item.Suite))
            return new UsersErrors.ValidationError();
        
        if (string.IsNullOrWhiteSpace(item.City))
            return new UsersErrors.ValidationError();
        
        if (string.IsNullOrWhiteSpace(item.ZipCode))
            return new UsersErrors.ValidationError();

        if (GeoCheckEmptyOrWhiteSpace(item.Geo).IsFailure)
            return GeoCheckEmptyOrWhiteSpace(item.Geo); 

        return true;
    }

    private static Result<bool, DomainError> GeoCheckEmptyOrWhiteSpace(GeoDto item)
    {
        if (string.IsNullOrWhiteSpace(item.Lat))
            return new UsersErrors.ValidationError();
        if (string.IsNullOrWhiteSpace(item.Lng))
            return new UsersErrors.ValidationError();
        
        return true;
    }
}